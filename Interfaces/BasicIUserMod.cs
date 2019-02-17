using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public abstract class BasicIUserMod<T, L, R> : IUserMod, ILoadingExtension where T : BasicIUserMod<T, L, R>, new() where L : KlyteLocaleUtils<L, R> where R : KlyteResourceLoader<R>
    {

        public abstract string SimpleName { get; }
        public abstract void LoadSettings();
        public abstract void doLog(string fmt, params object[] args);
        public abstract void doErrorLog(string fmt, params object[] args);
        public abstract void TopSettingsUI(UIHelperExtension ext);

        public string Name => $"{SimpleName} {version}";
        public abstract string Description { get; }
        public abstract void OnCreated(ILoading loading);
        public abstract void OnLevelLoaded(LoadMode mode);
        public abstract void OnLevelUnloading();
        public abstract void OnReleased();

        public static string minorVersion => majorVersion + "." + typeof(T).Assembly.GetName().Version.Build;
        public static string majorVersion => typeof(T).Assembly.GetName().Version.Major + "." + typeof(T).Assembly.GetName().Version.Minor;
        public static string fullVersion => minorVersion + " r" + typeof(T).Assembly.GetName().Version.Revision;
        public static string version
        {
            get {
                if (typeof(T).Assembly.GetName().Version.Minor == 0 && typeof(T).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(T).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(T).Assembly.GetName().Version.Build > 0)
                {
                    return minorVersion;
                }
                else
                {
                    return majorVersion;
                }
            }
        }

        private static SavedBool m_debugMode = new SavedBool(Singleton<L>.instance.prefix + "DebugMode", Settings.gameSettingsFile, false, true);
        public bool needShowPopup;
        private static bool isLocaleLoaded = false;
        public static bool LocaleLoaded => isLocaleLoaded;

        private static bool m_isKlyteCommonsLoaded = false;
        public static bool IsKlyteCommonsEnabled()
        {
            if (!m_isKlyteCommonsLoaded)
            {
                try
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var assembly = (from a in assemblies
                                    where a.GetType("Klyte.Commons.KlyteCommonsMod") != null
                                    select a).SingleOrDefault();
                    if (assembly != null)
                    {
                        m_isKlyteCommonsLoaded = true;
                    }
                }
                catch { }
            }
            return m_isKlyteCommonsLoaded;
        }

        public static SavedBool debugMode => m_debugMode;
        private SavedString currentSaveVersion => new SavedString(Singleton<L>.instance.prefix + "SaveVersion", Settings.gameSettingsFile, "null", true);
        private SavedInt currentLanguageId => new SavedInt(Singleton<L>.instance.prefix + "Language", Settings.gameSettingsFile, 0, true);
        public static bool isCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;




        protected void Construct()
        {
            Debug.LogWarningFormat(Singleton<L>.instance.prefix + "v" + majorVersion + " LOADING ");
            LoadSettings();
            Debug.LogWarningFormat(Singleton<L>.instance.prefix + "v" + majorVersion + " SETTING FILES");
            if (m_debugMode.value)
                Debug.LogWarningFormat("currentSaveVersion.value = {0}, fullVersion = {1}", currentSaveVersion.value, fullVersion);
            if (currentSaveVersion.value != fullVersion)
            {
                needShowPopup = true;
            }
            LocaleManager.eventLocaleChanged += new LocaleManager.LocaleChangedHandler(autoLoadLocale);
            loadLocale(false);
        }

        public virtual void OnSettingsUI(UIHelperBase helperDefault)
        {
            UIHelperExtension helper = new UIHelperExtension((UIHelper)helperDefault);
            void ev()
            {

                doLog("End asLoading Options");
                foreach (Transform child in helper.self.transform)
                {
                    GameObject.Destroy(child?.gameObject);
                }


                doLog("End Loadinasdadg Options");
                helper.self.eventVisibilityChanged += delegate (UIComponent component, bool b)
                {
                    if (b)
                    {
                        showVersionInfoPopup();
                    }
                };


                doLog("End dadadasdLoading Options");
                TopSettingsUI(helper);


                doLog("End Loadasdadadasdaading Options");
                UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("KCM_BETAS_EXTRA_INFO"));
                group9.AddDropdownLocalized("KCM_MOD_LANG", Singleton<L>.instance.getLanguageIndex(), currentLanguageId.value, delegate (int idx)
                 {
                     currentLanguageId.value = idx;
                     loadLocale(true);
                 });
                group9.AddCheckbox(Locale.Get("KCM_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
                group9.AddLabel(String.Format(Locale.Get("KCM_VERSION_SHOW"), fullVersion));
                if (typeof(R) != typeof(KCResourceLoader)) group9.AddLabel(Locale.Get("KCM_ORIGINAL_TLM_VERSION") + " " + string.Join(".", Singleton<R>.instance.loadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
                group9.AddButton(Locale.Get("KCM_RELEASE_NOTES"), delegate ()
                {
                    showVersionInfoPopup(true);
                });

                doLog("End Loading Options");
            }
            eventOnLoadLocaleEnd = null;
            eventOnLoadLocaleEnd += ev;
            loadLocale(false);

        }


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
                            string title = $"{SimpleName} v{version}";
                            string notes = Singleton<R>.instance.loadResourceString("UI.VersionNotes.txt");
                            string text = $"{SimpleName} was updated! Release notes:\r\n\r\n" + notes;
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

        public void autoLoadLocale()
        {
            if (currentLanguageId.value == 0)
            {
                loadLocale(false);
            }
        }
        public void loadLocale(bool force)
        {
            if (SingletonLite<LocaleManager>.exists && IsKlyteCommonsEnabled() && (!isLocaleLoaded || force))
            {
                Singleton<L>.instance.loadLocale(currentLanguageId.value == 0 ? SingletonLite<LocaleManager>.instance.language : Singleton<L>.instance.getSelectedLocaleByIndex(currentLanguageId.value), force);
                if (!isLocaleLoaded)
                {
                    isLocaleLoaded = true;
                    eventOnLoadLocaleEnd?.Invoke();
                }
            }
        }
        private delegate void OnLocaleLoadedFirstTime();
        private event OnLocaleLoadedFirstTime eventOnLoadLocaleEnd;

        private bool m_loaded = false;



        UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {
            Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            { // LoadTexture
                tex.LoadImage(Singleton<R>.instance.loadResourceData(textureFile));
                tex.Apply(true, true);
            }
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            { // Setup atlas
                Material material = (Material)Material.Instantiate(baseMaterial);
                material.mainTexture = tex;
                atlas.material = material;
                atlas.name = atlasName;
            }
            // Add sprites
            for (int i = 0; i < spriteNames.Length; ++i)
            {
                float uw = 1.0f / spriteNames.Length;
                var spriteInfo = new UITextureAtlas.SpriteInfo()
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };
                atlas.AddSprite(spriteInfo);
            }
            return atlas;
        }


    }

}
