using ColossalFramework;
using ColossalFramework.Globalization;
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
        public abstract string IconName { get; }
        public virtual bool UseGroup9 => true;
        public virtual void DoLog(string fmt, params object[] args) => LogUtils.DoLog(fmt, args);
        public virtual void DoErrorLog(string fmt, params object[] args) => LogUtils.DoErrorLog(fmt, args);
        public virtual void TopSettingsUI(UIHelperExtension ext) { }

        private GameObject m_topObj;
        public Transform RefTransform => m_topObj?.transform;

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
            Redirector.UnpatchAll();
            PatchesApply();
        }
        public virtual void OnReleased()
        {

            PluginManager.instance.eventPluginsStateChanged -= SearchIncompatibilitiesModal;
        }

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

            if (Locale.Get("K45_TEST_UP") != "OK")
            {
                KlyteMonoUtils.CreateElement<KlyteLocaleManager>(new GameObject(typeof(U).Name).transform);
                if (Locale.Get("K45_TEST_UP") != "OK")
                {
                    LogUtils.DoErrorLog("CAN'T LOAD LOCALE!!!!!");
                }
                LocaleManager.eventLocaleChanged += KlyteLocaleManager.ReloadLanguage;
                m_showLangDropDown = true;
            }
            foreach (string lang in KlyteLocaleManager.locales)
            {
                string content = KlyteResourceLoader.LoadResourceString($"UI.i18n.{lang}.properties");
                if (content != null)
                {
                    File.WriteAllText($"{KlyteLocaleManager.m_translateFilesPath}{lang}{Path.DirectorySeparatorChar}1_{Assembly.GetExecutingAssembly().GetName().Name}.txt", content);
                }
                content = KlyteResourceLoader.LoadResourceString($"commons.UI.i18n.{lang}.properties");
                if (content != null)
                {
                    File.WriteAllText($"{KlyteLocaleManager.m_translateFilesPath}{lang}{Path.DirectorySeparatorChar}0_common.txt", content);
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
            TextureAtlasUtils.LoadIamgesFromResources("commons.UI.Images", ref newSprites);
            TextureAtlasUtils.LoadIamgesFromResources("UI.Images", ref newSprites);
            LogUtils.DoLog($"ADDING {newSprites.Count} sprites!");
            TextureAtlasUtils.RegenerateDefaultTextureAtlas(newSprites);


            helper.Self.eventVisibilityChanged += delegate (UIComponent component, bool b)
            {
                if (b)
                {
                    ShowVersionInfoPopup();
                }
            };

            TopSettingsUI(helper);

            if (UseGroup9)
            {
                CreateGroup9(helper);
            }

            ShowVersionInfoPopup();
            SearchIncompatibilitiesModal();
            LogUtils.DoLog("End Loading Options");
        }



        protected void CreateGroup9(UIHelperExtension helper)
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

        public bool ShowVersionInfoPopup(bool force = false)
        {
            if (needShowPopup || force)
            {
                try
                {
                    string title = $"{SimpleName} v{Version}";
                    string notes = KlyteResourceLoader.LoadResourceString("UI.VersionNotes.txt");
                    string text = $"{SimpleName} was updated! Release notes:\n\n{notes}\n\n<k45symbol K45_HexagonIcon_NOBORDER,5e35b1,K> Current Version: <color #FFFF00>{FullVersion}</color>";

                    ShowModal(new BindProperties()
                    {
                        icon = IconName,
                        showButton1 = true,
                        textButton1 = "Okay!",
                        messageAlign = UIHorizontalAlignment.Left,
                        title = title,
                        message = text,
                    }, (x) => true);

                    needShowPopup = false;
                    CurrentSaveVersion.value = FullVersion;
                    return true;
                }
                catch (Exception e)
                {
                    DoErrorLog("showVersionInfoPopup ERROR {0} {1}\n{2}", e.GetType(), e.Message,e.StackTrace);
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
                else
                {
                    LogUtils.DoLog("PANEL NOT FOUND!!!!");
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
                return VerifyModsEnabled(IncompatibleModList, IncompatibleDllModList);
            }
        }
        public static Dictionary<ulong, string> VerifyModsEnabled(List<ulong> modIds, List<string> modsDlls) => Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
            pi.assemblyCount > 0
            && pi.isEnabled
            && (modIds.Contains(pi.publishedFileID.AsUInt64) || pi.GetAssemblies().Where(x => modsDlls.Contains(x.GetName().Name)).Count() > 0)
        ).ToDictionary(x => x.publishedFileID.AsUInt64, x => ((IUserMod)x.userModInstance).Name);
        public void OnViewStart()
        {
            ShowVersionInfoPopup();
            SearchIncompatibilitiesModal();
            ExtraOnViewStartActions();
        }

        protected virtual void ExtraOnViewStartActions() { }

        public virtual List<ulong> IncompatibleModList { get; } = new List<ulong>();
        public virtual List<string> IncompatibleDllModList { get; } = new List<string>();

    }

}
