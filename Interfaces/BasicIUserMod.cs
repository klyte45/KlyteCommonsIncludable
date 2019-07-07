using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public abstract class BasicIUserMod<U, R, C, A, T> : IUserMod, ILoadingExtension
        where U : BasicIUserMod<U, R, C, A, T>, new()
        where R : KlyteResourceLoader<R>
        where C : MonoBehaviour
        where A : TextureAtlasDescriptor<A, R>
        where T : UICustomControl
    {

        public abstract string SimpleName { get; }
        public virtual bool UseGroup9 => true;
        public abstract void LoadSettings();
        public abstract void DoLog(string fmt, params object[] args);
        public abstract void DoErrorLog(string fmt, params object[] args);
        public abstract void TopSettingsUI(UIHelperExtension ext);

        private GameObject m_topObj;
        public Transform RefTransform => m_topObj?.transform;

        protected virtual ModTab? Tab => null;
        protected virtual float? TabWidth => null;


        public string Name => $"{SimpleName} {Version}";
        public abstract string Description { get; }

        private C m_controller;
        public C Controller => m_controller;

        public void OnCreated(ILoading loading)
        {

        }
        public void OnLevelLoaded(LoadMode mode)
        {
            m_topObj = new GameObject(typeof(U).Name);
            var typeTarg = typeof(IRedirectable);
            var instances = ReflectionUtils.GetSubtypesRecursive(typeTarg, typeof(U));
            DoLog($"{SimpleName} Redirectors: {instances.Count()}");
            foreach (Type t in instances)
            {
                m_topObj.AddComponent(t);
            }
            if (typeof(C) != typeof(MonoBehaviour))
            {
                m_controller = m_topObj.AddComponent<C>();
            }
            if (Tab != null)
            {
                KlyteModsPanel.Instance.AddTab((ModTab)Tab, typeof(T), Singleton<A>.instance.Atlas, Singleton<A>.instance.SpriteNames[0], GeneralName, (x, y) => { if (y) ShowVersionInfoPopup(); }, TabWidth);
            }

        }

        public string GeneralName => $"{SimpleName} (v{Version})";

        public void OnLevelUnloading()
        {
            if (typeof(U).Assembly.GetName().Version.Revision != 9999)
            {
                Application.Quit();
            }
        }
        public virtual void OnReleased()
        {
            OnLevelUnloading();
        }

        public static string MinorVersion => MajorVersion + "." + typeof(U).Assembly.GetName().Version.Build;
        public static string MajorVersion => typeof(U).Assembly.GetName().Version.Major + "." + typeof(U).Assembly.GetName().Version.Minor;
        public static string FullVersion => MinorVersion + " r" + typeof(U).Assembly.GetName().Version.Revision;
        public static string Version
        {
            get {
                if (typeof(U).Assembly.GetName().Version.Minor == 0 && typeof(U).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(U).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(U).Assembly.GetName().Version.Build > 0)
                {
                    return MinorVersion;
                }
                else
                {
                    return MajorVersion;
                }
            }
        }

        private static readonly SavedBool m_debugMode = new SavedBool(KlyteLocaleManager.m_defaultPrefixInGame + "DebugMode", Settings.gameSettingsFile, false, true);
        public bool needShowPopup;

        public static SavedBool DebugMode => m_debugMode;
        private SavedString CurrentSaveVersion => new SavedString(KlyteLocaleManager.m_defaultPrefixInGame + "SaveVersion", Settings.gameSettingsFile, "null", true);
        public static bool IsCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;

        public static U Instance { get; set; }

        protected void Construct()
        {
            Instance = this as U;
            Debug.LogWarningFormat(KlyteLocaleManager.m_defaultPrefixInGame + "v" + MajorVersion + " LOADING ");
            LoadSettings();
            Debug.LogWarningFormat(KlyteLocaleManager.m_defaultPrefixInGame + "v" + MajorVersion + " SETTING FILES");
            if (m_debugMode.value)
                Debug.LogWarningFormat("currentSaveVersion.value = {0}, fullVersion = {1}", CurrentSaveVersion.value, FullVersion);
            if (CurrentSaveVersion.value != FullVersion)
            {
                needShowPopup = true;
            }
        }

        UIComponent m_onSettingsUiComponent;

        public void OnSettingsUI(UIHelperBase helperDefault)
        {
            m_onSettingsUiComponent = new UIHelperExtension((UIHelper)helperDefault).Self ?? m_onSettingsUiComponent;
            void ev()
            {
                if (m_onSettingsUiComponent != null)
                    DoWithSettingsUI(new UIHelperExtension(m_onSettingsUiComponent));
            }
            if (Locale.Get("K45_TEST_UP") != "OK")
            {
                var localeMan = KlyteMonoUtils.CreateElement<KlyteLocaleManager>(null);
                if (Locale.Get("K45_TEST_UP") != "OK")
                {
                    LogUtils.DoErrorLog("CAN'T LOAD LOCALE!!!!!");
                }
                LocaleManager.eventLocaleChanged += localeMan.ReloadLanguage;
            }
            var localeManager = GameObject.FindObjectOfType<KlyteLocaleManager>();
            foreach (var lang in KlyteLocaleManager.locales)
            {
                var content = Singleton<R>.instance.LoadResourceString($"UI.i18n.{lang}.properties");
                if (content != null)
                {
                    File.WriteAllText($"{KlyteLocaleManager.m_translateFilesPath}{lang}{Path.DirectorySeparatorChar}0_{Assembly.GetExecutingAssembly().GetName().Name}.txt", content);
                }
            }
            localeManager.ReloadLanguage();
            ev();
        }

        private void DoWithSettingsUI(UIHelperExtension helper)
        {
            foreach (Transform child in helper.Self?.transform)
            {
                GameObject.Destroy(child?.gameObject);
            }

            helper.Self.eventVisibilityChanged += delegate (UIComponent component, bool b)
            {
                if (b)
                {
                    ShowVersionInfoPopup();
                }
            };

            TopSettingsUI(helper);

            if (UseGroup9) CreateGroup9(helper);

            DoLog("End Loading Options");
        }

        protected void CreateGroup9(UIHelperExtension helper)
        {
            UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("K45_BETAS_EXTRA_INFO"));
            Group9SettingsUI(group9);

            group9.AddCheckbox(Locale.Get("K45_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
            group9.AddLabel(String.Format(Locale.Get("K45_VERSION_SHOW"), FullVersion));
            if (typeof(R) != typeof(KCResourceLoader)) group9.AddLabel(Locale.Get("K45_ORIGINAL_TLM_VERSION") + " " + string.Join(".", Singleton<R>.instance.LoadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
            group9.AddButton(Locale.Get("K45_RELEASE_NOTES"), delegate ()
            {
                ShowVersionInfoPopup(true);
            });
        }

        public virtual void Group9SettingsUI(UIHelperExtension group9) { }

        public bool ShowVersionInfoPopup(bool force = false)
        {
            if (needShowPopup || force)
            {
                try
                {
                    UIComponent uIComponent = UIView.library.ShowModal("ExceptionPanel");
                    if (uIComponent != null)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        BindPropertyByKey component = uIComponent.GetComponent<BindPropertyByKey>();
                        if (component != null)
                        {
                            string title = $"{SimpleName.Replace("&", "and")} v{Version}";
                            string notes = Singleton<R>.instance.LoadResourceString("UI.VersionNotes.txt");
                            string text = $"{SimpleName.Replace("&", "and")} was updated! Release notes:\r\n\r\n" + notes;
                            string img = "IconMessage";
                            component.SetProperties(TooltipHelper.Format(new string[]
                            {
                            "title",
                            title,
                            "message",
                            text,
                            "img",
                            img
                            }));
                            needShowPopup = false;
                            CurrentSaveVersion.value = FullVersion;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        DoLog("PANEL NOT FOUND!!!!");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    DoErrorLog("showVersionInfoPopup ERROR {0} {1}", e.GetType(), e.Message);
                }
            }
            return false;
        }

    }

}
