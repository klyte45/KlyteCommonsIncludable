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
        public abstract void doLog(string fmt, params object[] args);
        public abstract void doErrorLog(string fmt, params object[] args);
        public abstract void TopSettingsUI(UIHelperExtension ext);

        private GameObject topObj;
        public Transform refTransform => topObj?.transform;

        protected virtual ModTab? Tab => null;
        protected virtual float? TabWidth => null;


        public string Name => $"{SimpleName} {version}";
        public abstract string Description { get; }

        private C m_controller;
        public C controller => m_controller;

        public void OnCreated(ILoading loading)
        {

        }
        public void OnLevelLoaded(LoadMode mode)
        {
            topObj = new GameObject(typeof(U).Name);
            var typeTarg = typeof(Redirector<>);
            var instances = KlyteUtils.GetSubtypesRecursive(typeTarg, typeof(U));
            doLog($"{SimpleName} Redirectors: {instances.Count()}");
            foreach (Type t in instances)
            {
                topObj.AddComponent(t);
            }
            if (typeof(C) != typeof(MonoBehaviour))
            {
                m_controller = topObj.AddComponent<C>();
            }
            if (Tab != null)
            {
                KlyteModsPanel.instance.AddTab((ModTab)Tab, typeof(T), Singleton<A>.instance.atlas, Singleton<A>.instance.SpriteNames[0], GeneralName, (x, y) => { if (y) showVersionInfoPopup(); }, TabWidth);
            }

        }

        public string GeneralName => $"{SimpleName} (v{version})";

        public void OnLevelUnloading()
        {
            var typeTarg = typeof(Redirector<>);
            var instances = KlyteUtils.GetSubtypesRecursive(typeTarg, typeof(U));
            doLog($"{SimpleName} Redirectors: {instances.Count()}");
            foreach (Type t in instances)
            {
                GameObject.Destroy((Redirector)KlyteUtils.GetPrivateStaticField("instance", t));
            }
            GameObject.Destroy(topObj);
            typeTarg = typeof(Singleton<>);
            instances = KlyteUtils.GetSubtypesRecursive(typeTarg, typeof(U));

            foreach (Type t in instances)
            {
                GameObject.Destroy(((MonoBehaviour)KlyteUtils.GetPrivateStaticProperty("instance", t)));
            }
        }
        public virtual void OnReleased()
        {
            OnLevelUnloading();
        }

        public static string minorVersion => majorVersion + "." + typeof(U).Assembly.GetName().Version.Build;
        public static string majorVersion => typeof(U).Assembly.GetName().Version.Major + "." + typeof(U).Assembly.GetName().Version.Minor;
        public static string fullVersion => minorVersion + " r" + typeof(U).Assembly.GetName().Version.Revision;
        public static string version
        {
            get {
                if (typeof(U).Assembly.GetName().Version.Minor == 0 && typeof(U).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(U).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(U).Assembly.GetName().Version.Build > 0)
                {
                    return minorVersion;
                }
                else
                {
                    return majorVersion;
                }
            }
        }

        private static U m_instance;
        public static U instance => m_instance;

        private static SavedBool m_debugMode = new SavedBool(Singleton<L>.instance.prefix + "DebugMode", Settings.gameSettingsFile, false, true);
        public bool needShowPopup;
        private static bool isLocaleLoaded = false;
        public static bool LocaleLoaded => isLocaleLoaded;


        public static SavedBool debugMode => m_debugMode;
        private SavedString currentSaveVersion => new SavedString(Singleton<L>.instance.prefix + "SaveVersion", Settings.gameSettingsFile, "null", true);
        public static bool isCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;




        protected void Construct()
        {
            m_instance = this as U;
            Debug.LogWarningFormat(Singleton<L>.instance.prefix + "v" + majorVersion + " LOADING ");
            LoadSettings();
            Debug.LogWarningFormat(Singleton<L>.instance.prefix + "v" + majorVersion + " SETTING FILES");
            if (m_debugMode.value)
                Debug.LogWarningFormat("currentSaveVersion.value = {0}, fullVersion = {1}", currentSaveVersion.value, fullVersion);
            if (currentSaveVersion.value != fullVersion)
            {
                needShowPopup = true;
            }
        }

        UIComponent onSettingsUiComponent;

        public void OnSettingsUI(UIHelperBase helperDefault)
        {
            onSettingsUiComponent = new UIHelperExtension((UIHelper)helperDefault).self ?? onSettingsUiComponent;
            void ev()
            {
                if (onSettingsUiComponent != null)
                    DoWithSettingsUI(new UIHelperExtension(onSettingsUiComponent));
            }
            eventOnLoadLocaleEnd += ev;
            if (typeof(U) == typeof(KlyteCommonsMod))
            {
                loadLocale(false);
            }
            else
            {
                KlyteCommonsMod.instance.eventOnLoadLocaleEnd += delegate ()
                {
                    loadLocale(true);
                };
            }
            if (KlyteCommonsMod.isLocaleLoaded)
            {
                loadLocale(false);
                ev();
            }

        }

        private void DoWithSettingsUI(UIHelperExtension helper)
        {
            foreach (Transform child in helper.self?.transform)
            {
                GameObject.Destroy(child?.gameObject);
            }

            helper.self.eventVisibilityChanged += delegate (UIComponent component, bool b)
            {
                if (b)
                {
                    showVersionInfoPopup();
                }
            };

            TopSettingsUI(helper);

            if (UseGroup9) CreateGroup9(helper);

            doLog("End Loading Options");
        }

        protected void CreateGroup9(UIHelperExtension helper)
        {
            UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("KCM_BETAS_EXTRA_INFO"));
            Group9SettingsUI(group9);

            group9.AddCheckbox(Locale.Get("KCM_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
            group9.AddLabel(String.Format(Locale.Get("KCM_VERSION_SHOW"), fullVersion));
            if (typeof(R) != typeof(KCResourceLoader)) group9.AddLabel(Locale.Get("KCM_ORIGINAL_TLM_VERSION") + " " + string.Join(".", Singleton<R>.instance.loadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
            group9.AddButton(Locale.Get("KCM_RELEASE_NOTES"), delegate ()
            {
                showVersionInfoPopup(true);
            });
        }

        public virtual void Group9SettingsUI(UIHelperExtension group9) { }

        public bool showVersionInfoPopup(bool force = false)
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
                            string title = $"{SimpleName.Replace("&", "and")} v{version}";
                            string notes = Singleton<R>.instance.loadResourceString("UI.VersionNotes.txt");
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
                            currentSaveVersion.value = fullVersion;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        doLog("PANEL NOT FOUND!!!!");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    doErrorLog("showVersionInfoPopup ERROR {0} {1}", e.GetType(), e.Message);
                }
            }
            return false;
        }

        public void loadLocale(bool force)
        {
            if (!isLocaleLoaded || force)
            {
                Singleton<L>.instance.loadCurrentLocale(force);
                eventOnLoadLocaleEnd?.Invoke();
                isLocaleLoaded = true;
            }
        }
        private delegate void OnLocaleLoadedFirstTime();
        private event OnLocaleLoadedFirstTime eventOnLoadLocaleEnd;

    }

}
