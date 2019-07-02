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
    public abstract class BasicIUserMod<U, L, R, C, A, T> : IUserMod, ILoadingExtension
        where U : BasicIUserMod<U, L, R, C, A, T>, new()
        where L : KlyteLocaleUtils<L, R>
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
            var typeTarg = typeof(Redirector<>);
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
            var typeTarg = typeof(Redirector<>);
            var instances = ReflectionUtils.GetSubtypesRecursive(typeTarg, typeof(U));
            DoLog($"{SimpleName} Redirectors: {instances.Count()}");
            foreach (Type t in instances)
            {
                GameObject.Destroy((Redirector)ReflectionUtils.GetPrivateStaticField("instance", t));
            }
            GameObject.Destroy(m_topObj);
            typeTarg = typeof(Singleton<>);
            instances = ReflectionUtils.GetSubtypesRecursive(typeTarg, typeof(U));

            foreach (Type t in instances)
            {
                GameObject.Destroy(((MonoBehaviour)ReflectionUtils.GetPrivateStaticProperty("instance", t)));
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

        private static U m_instance;
        public static U Instance => m_instance;

        private static readonly SavedBool m_debugMode = new SavedBool(Singleton<L>.instance.Prefix + "DebugMode", Settings.gameSettingsFile, false, true);
        public bool needShowPopup;
        private static bool m_isLocaleLoaded = false;
        public static bool LocaleLoaded => m_isLocaleLoaded;


        public static SavedBool DebugMode => m_debugMode;
        private SavedString CurrentSaveVersion => new SavedString(Singleton<L>.instance.Prefix + "SaveVersion", Settings.gameSettingsFile, "null", true);
        public static bool IsCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;




        protected void Construct()
        {
            m_instance = this as U;
            Debug.LogWarningFormat(Singleton<L>.instance.Prefix + "v" + MajorVersion + " LOADING ");
            LoadSettings();
            Debug.LogWarningFormat(Singleton<L>.instance.Prefix + "v" + MajorVersion + " SETTING FILES");
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
            EventOnLoadLocaleEnd += ev;
            if (typeof(U) == typeof(KlyteCommonsMod))
            {
                LoadLocale(false);
            }
            else
            {
                KlyteCommonsMod.Instance.EventOnLoadLocaleEnd += delegate ()
                {
                    LoadLocale(true);
                };
            }
            if (KlyteCommonsMod.m_isLocaleLoaded)
            {
                LoadLocale(false);
                ev();
            }

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
            UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("KCM_BETAS_EXTRA_INFO"));
            Group9SettingsUI(group9);

            group9.AddCheckbox(Locale.Get("KCM_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
            group9.AddLabel(String.Format(Locale.Get("KCM_VERSION_SHOW"), FullVersion));
            if (typeof(R) != typeof(KCResourceLoader)) group9.AddLabel(Locale.Get("KCM_ORIGINAL_TLM_VERSION") + " " + string.Join(".", Singleton<R>.instance.LoadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
            group9.AddButton(Locale.Get("KCM_RELEASE_NOTES"), delegate ()
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

        public void LoadLocale(bool force)
        {
            if (!m_isLocaleLoaded || force)
            {
                Singleton<L>.instance.LoadCurrentLocale(force);
                EventOnLoadLocaleEnd?.Invoke();
                m_isLocaleLoaded = true;
            }
        }
        private delegate void OnLocaleLoadedFirstTime();
        private event OnLocaleLoadedFirstTime EventOnLoadLocaleEnd;

    }

}
