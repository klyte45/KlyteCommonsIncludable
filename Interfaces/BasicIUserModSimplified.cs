using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static ColossalFramework.UI.UITextureAtlas;
using static Klyte.Commons.Utils.K45DialogControl;

namespace Klyte.Commons.Interfaces
{

    public abstract class BasicIUserModSimplified<U, C> : IUserMod, ILoadingExtension, IViewStartActions
        where U : BasicIUserModSimplified<U, C>, new()
        where C : BaseController<U, C>
    {
        public abstract string SimpleName { get; }
        public virtual string IconName { get; } = $"K45_{CommonProperties.Acronym}_Icon";
        public virtual bool UseGroup9 => true;
        public virtual void DoLog(string fmt, params object[] args) => LogUtils.DoLog(fmt, args);
        public virtual void DoErrorLog(string fmt, params object[] args) => LogUtils.DoErrorLog(fmt, args);
        public virtual void TopSettingsUI(UIHelperExtension ext) { }

        private GameObject m_topObj;
        public Transform RefTransform => m_topObj?.transform;

        private static ulong m_modId;

        public static ulong ModId
        {
            get
            {
                if (m_modId == 0)
                {
                    m_modId = Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
                 pi.assemblyCount > 0
                 && pi.isEnabled
                 && pi.GetAssemblies().Where(x => x == typeof(U).Assembly).Count() > 0
             ).Select(x => x?.publishedFileID.AsUInt64 ?? ulong.MaxValue).Min();
                }
                return m_modId;
            }
        }

        private static string m_rootFolder;

        public static string RootFolder
        {
            get
            {
                if (m_rootFolder == null)
                {
                    m_rootFolder = Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
                 pi.assemblyCount > 0
                 && pi.isEnabled
                 && pi.GetAssemblies().Where(x => x == typeof(U).Assembly).Count() > 0
             ).FirstOrDefault()?.modPath;
                }
                return m_rootFolder;
            }
        }
        public string Name => $"{SimpleName} {Version}";
        public abstract string Description { get; }
        public static C Controller { get; private set; }

        public virtual void OnCreated(ILoading loading)
        {
            if (loading == null || (!loading.loadingComplete && !IsValidLoadMode(loading)))
            {
                Redirector.UnpatchAll();
            }
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            OnLevelLoadedInherit(mode);
            OnLevelLoadingInternal();
        }

        protected virtual void OnLevelLoadedInherit(LoadMode mode)
        {
            if (IsValidLoadMode(mode))
            {
                if (!typeof(C).IsGenericType)
                {
                    m_topObj = GameObject.Find(typeof(U).Name) ?? new GameObject(typeof(U).Name);
                    Controller = m_topObj.AddComponent<C>();
                }
                SimulationManager.instance.StartCoroutine(LevelUnloadBinds());
                ShowVersionInfoPopup();
                SearchIncompatibilitiesModal();
            }
            else
            {
                LogUtils.DoWarnLog($"Invalid load mode: {mode}. The mod will not be loaded!");
                Redirector.UnpatchAll();
            }
        }

        private IEnumerator LevelUnloadBinds()
        {
            yield return 0;
            UIButton toMainMenuButton = GameObject.Find("ToMainMenu")?.GetComponent<UIButton>();
            if (toMainMenuButton != null)
            {
                toMainMenuButton.eventClick += (x, y) =>
                {
                    GameObject.FindObjectOfType<ToolsModifierControl>().CloseEverything();
                    ExtraUnloadBinds();
                };
            }
        }

        protected virtual void ExtraUnloadBinds() { }

        protected virtual void OnLevelLoadingInternal()
        {

        }

        protected virtual bool IsValidLoadMode(ILoading loading) => loading?.currentMode == AppMode.Game;
        protected virtual bool IsValidLoadMode(LoadMode mode) => mode == LoadMode.LoadGame || mode == LoadMode.LoadScenario || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario;
        public string GeneralName => $"{SimpleName} (v{Version})";

        public void OnLevelUnloading()
        {
            Controller = null;
            Redirector.UnpatchAll();
            PatchesApply();
        }
        public virtual void OnReleased() => PluginManager.instance.eventPluginsStateChanged -= SearchIncompatibilitiesModal;

        protected void PatchesApply()
        {
            Redirector.PatchAll();
            OnPatchesApply();
        }

        protected virtual void OnPatchesApply() { }

        public void OnEnabled()
        {
            if (CurrentSaveVersion.value != FullVersion)
            {
                needShowPopup = true;
            }
            FileUtils.EnsureFolderCreation(CommonProperties.ModRootFolder);
            PatchesApply();
        }

        public void OnDisabled() => Redirector.UnpatchAll();

        public static string MinorVersion => MajorVersion + "." + typeof(U).Assembly.GetName().Version.Build;
        public static string MajorVersion => typeof(U).Assembly.GetName().Version.Major + "." + typeof(U).Assembly.GetName().Version.Minor;
        public static string FullVersion => MinorVersion + " r" + typeof(U).Assembly.GetName().Version.Revision;
        public static string Version
        {
            get
            {
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

        public bool needShowPopup;

        public static SavedBool DebugMode { get; } = new SavedBool(CommonProperties.Acronym + "_DebugMode", Settings.gameSettingsFile, false, true);
        private SavedString CurrentSaveVersion { get; } = new SavedString(CommonProperties.Acronym + "SaveVersion", Settings.gameSettingsFile, "null", true);
        public static bool IsCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;

        public static U m_instance = new U();
        public static U Instance => m_instance;

        private UIComponent m_onSettingsUiComponent;
        private bool m_showLangDropDown = false;

        public void OnSettingsUI(UIHelperBase helperDefault)
        {

            m_onSettingsUiComponent = new UIHelperExtension((UIHelper)helperDefault).Self ?? m_onSettingsUiComponent;

            if (!Locale.Exists(KlyteLocaleManager.m_defaultTestKey) || Locale.Get(KlyteLocaleManager.m_defaultModControllingKey) == CommonProperties.ModName)
            {
                if (Locale.Get(KlyteLocaleManager.m_defaultModControllingKey) != CommonProperties.ModName)
                {
                    KlyteMonoUtils.CreateElement<KlyteLocaleManager>(new GameObject(typeof(U).Name).transform);
                    if (!Locale.Exists(KlyteLocaleManager.m_defaultTestKey))
                    {
                        LogUtils.DoErrorLog("CAN'T LOAD LOCALE!!!!!");
                    }
                    LocaleManager.eventLocaleChanged += KlyteLocaleManager.ReloadLanguage;
                }

                m_showLangDropDown = true;
            }
            foreach (string lang in KlyteLocaleManager.locales)
            {
                string content = KlyteResourceLoader.LoadResourceString($"UI.i18n.{lang}.properties");
                FileUtils.EnsureFolderCreation($"{KlyteLocaleManager.m_translateFilesPath}{lang}");
                if (content != null)
                {
                    File.WriteAllText($"{KlyteLocaleManager.m_translateFilesPath}{lang}{Path.DirectorySeparatorChar}1_{Assembly.GetExecutingAssembly().GetName().Name}.txt", content);
                }
                content = KlyteResourceLoader.LoadResourceString($"commons.UI.i18n.{lang}.properties");
                if (content != null)
                {
                    File.WriteAllText($"{KlyteLocaleManager.m_translateFilesPath}{lang}{Path.DirectorySeparatorChar}0_common_{K45DialogControl.VERSION}.txt", content);
                }

            }
            KlyteLocaleManager.ReloadLanguage(true);
            DoWithSettingsUI(new UIHelperExtension(m_onSettingsUiComponent));
        }

        private void DoWithSettingsUI(UIHelperExtension helper)
        {
            foreach (Transform child in helper.Self?.transform)
            {
                GameObject.Destroy(child?.gameObject);
            }

            var newSprites = new List<SpriteInfo>();
            TextureAtlasUtils.LoadImagesFromResources("commons.UI.Images", ref newSprites);
            TextureAtlasUtils.LoadImagesFromResources("UI.Images", ref newSprites);
            LogUtils.DoLog($"ADDING {newSprites.Count} sprites!");
            TextureAtlasUtils.RegenerateDefaultTextureAtlas(newSprites);

            TopSettingsUI(helper);

            if (UseGroup9)
            {
                CreateGroup9(helper);
            }

            LogUtils.DoLog("End Loading Options");
        }



        protected virtual void CreateGroup9(UIHelperExtension helper)
        {
            UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("K45_BETAS_EXTRA_INFO"));
            Group9SettingsUI(group9);

            group9.AddCheckbox(Locale.Get("K45_DEBUG_MODE"), DebugMode.value, delegate (bool val)
            { DebugMode.value = val; });
            group9.AddLabel(string.Format(Locale.Get("K45_VERSION_SHOW"), FullVersion));
            group9.AddButton(Locale.Get("K45_RELEASE_NOTES"), delegate ()
            {
                ShowVersionInfoPopup(true);
            });
            group9.AddButton("Report-a-bug helper", () => K45DialogControl.ShowModal(new K45DialogControl.BindProperties()
            {
                icon = IconName,
                title = "Report-a-bug helper",
                message = "If you find any problem with this mod, please send me the output_log.txt (or player.log on Mac/Linux) in the mod Workshop page. If applies, a printscreen can help too to make a better guess about what is happening wrong here...\n\n" +
                         "There's a link for a Workshop guide by <color #008800>aubergine18</color> explaining how to find your log file, depending of OS you're using.\nFeel free to create a topic at Workshop or just leave a comment linking your files.",
                showButton1 = true,
                textButton1 = "Okay...",
                showButton2 = true,
                textButton2 = "Go to the guide",
                showButton3 = true,
                textButton3 = "Go to mod page"
            }, (x) =>
            {
                if (x == 2)
                {
                    ColossalFramework.Utils.OpenUrlThreaded("https://steamcommunity.com/sharedfiles/filedetails/?id=463645931");
                    return false;
                }
                if (x == 3)
                {
                    ColossalFramework.Utils.OpenUrlThreaded("https://steamcommunity.com/sharedfiles/filedetails/?id=" + ModId);
                    return false;
                }
                return true;
            }));

            if (m_showLangDropDown)
            {
                UIDropDown dd = null;
                dd = group9.AddDropdownLocalized("K45_MOD_LANG", (new string[] { "K45_GAME_DEFAULT_LANGUAGE" }.Concat(KlyteLocaleManager.locales.Select(x => $"K45_LANG_{x}")).Select(x => Locale.Get(x))).ToArray(), KlyteLocaleManager.GetLoadedLanguage(), delegate (int idx)
                {
                    KlyteLocaleManager.SaveLoadedLanguage(idx);
                    KlyteLocaleManager.ReloadLanguage();
                    KlyteLocaleManager.RedrawUIComponents();
                });
            }
            else
            {
                group9.AddLabel(string.Format(Locale.Get("K45_LANG_CTRL_MOD_INFO"), Locale.Get("K45_MOD_CONTROLLING_LOCALE")));
            }

        }

        public virtual void Group9SettingsUI(UIHelperExtension group9) { }

        protected virtual Tuple<string, string> GetButtonLink() => null;

        public bool ShowVersionInfoPopup(bool force = false)
        {
            if ((needShowPopup &&
                (SimulationManager.instance.m_metaData?.m_updateMode == SimulationManager.UpdateMode.LoadGame
                || SimulationManager.instance.m_metaData?.m_updateMode == SimulationManager.UpdateMode.NewGameFromMap
                || SimulationManager.instance.m_metaData?.m_updateMode == SimulationManager.UpdateMode.NewGameFromScenario
                || PackageManager.noWorkshop
                ))
                || force)
            {
                try
                {
                    string title = $"{SimpleName} v{Version}";
                    string notes = KlyteResourceLoader.LoadResourceString("UI.VersionNotes.txt");
                    var fullWidth = notes.StartsWith("<extended>");
                    if (fullWidth)
                    {
                        notes = notes.Substring("<extended>".Length);
                    }
                    string text = $"{SimpleName} was updated! Release notes:\n\n{notes}\n\n<sprite K45_K45Button> Current Version: <color #FFFF00>{FullVersion}</color>";
                    var targetUrl = GetButtonLink();
                    ShowModal(new BindProperties()
                    {
                        icon = IconName,
                        showClose = true,
                        showButton1 = true,
                        textButton1 = "Okay!",
                        showButton2 = true,
                        textButton2 = "See the news on the mod page at Workshop!",
                        showButton3 = !(targetUrl is null),
                        textButton3 = targetUrl?.First ?? "",
                        showButton4 = true,
                        textButton4 = "Follow Klyte45 on Twitter!",
                        showButton5 = true,
                        textButton5 = "Subscribe to Klyte45 channel on YouTube!",
                        messageAlign = UIHorizontalAlignment.Left,
                        useFullWindowWidth = fullWidth,
                        title = title,
                        message = text,
                    }, (x) =>
                    {
                        switch (x)
                        {
                            case 0:
                            case 1:
                                needShowPopup = false;
                                CurrentSaveVersion.value = FullVersion;
                                break;
                            case 2:
                                ColossalFramework.Utils.OpenUrlThreaded("https://steamcommunity.com/sharedfiles/filedetails/?id=" + ModId);
                                break;
                            case 3:
                                if (!(targetUrl is null))
                                {
                                    ColossalFramework.Utils.OpenUrlThreaded(targetUrl.Second);
                                }
                                break;
                            case 4:
                                ColossalFramework.Utils.OpenUrlThreaded("https://twitter.com/klyte45");
                                break;
                            case 5:
                                ColossalFramework.Utils.OpenUrlThreaded("https://youtube.com/klyte45");
                                break;

                        }
                        return x <= 1;
                    });

                    return true;
                }
                catch (Exception e)
                {
                    DoErrorLog("showVersionInfoPopup ERROR {0} {1}\n{2}", e.GetType(), e.Message, e.StackTrace);
                }
            }
            return false;
        }
        public void SearchIncompatibilitiesModal()
        {
            try
            {
                Dictionary<ulong, string> notes = SearchIncompatibilities();
                if (notes != null && notes.Count > 0)
                {
                    string title = $"{SimpleName} - Incompatibility report";
                    string text;
                    unchecked
                    {
                        text = $"Some conflicting mods were found active. Disable or unsubscribe them to make the <color>{SimpleName}</color> work properly." +
                            $"\n\n{string.Join("\n", notes.Select(x => $"\t -{x.Value} (id: {(x.Key == (ulong)-1 ? "<LOCAL>" : x.Key.ToString())})").ToArray())}" +
                            $"\n\nDisable or unsubscribe them at main menu and try again!";
                    }
                    ShowModal(new BindProperties()
                    {
                        icon = IconName,
                        showButton1 = true,
                        textButton1 = "Err... Okay!",
                        messageAlign = UIHorizontalAlignment.Left,
                        title = title,
                        message = text,
                    }, (x) => true);
                }
            }
            catch (Exception e)
            {
                DoErrorLog("SearchIncompatibilitiesModal ERROR {0} {1}\n{2}", e.GetType(), e.Message, e.StackTrace);
            }
        }

        public Dictionary<ulong, string> SearchIncompatibilities()
        {
            if (IncompatibleModList.Count == 0)
            {
                return null;
            }
            else
            {
                return PluginUtils.VerifyModsEnabled(IncompatibleModList, IncompatibleDllModList);
            }
        }
        public void OnViewStart() => ExtraOnViewStartActions();

        protected virtual void ExtraOnViewStartActions() { }

        protected virtual List<ulong> IncompatibleModList { get; } = new List<ulong>();
        protected virtual List<string> IncompatibleDllModList { get; } = new List<string>();

        private List<ulong> IncompatibleModListCommons { get; } = new List<ulong>();
        private List<string> IncompatibleDllModListCommons { get; } = new List<string>();


        public IEnumerable<ulong> IncompatibleModListAll => IncompatibleModListCommons.Union(IncompatibleModList);
        public IEnumerable<string> IncompatibleDllModListAll => IncompatibleDllModListCommons.Union(IncompatibleDllModList);

    }

}
