using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Overrides
{
    public class NonKCModsOverrides : Redirector<NonKCModsOverrides>
    {

        private static readonly string[] supportedClasses = new string[] {
            "Klyte.SuburbStyler.SSController, SuburbStyler"
        };

        #region Override gen

        public static bool CreateMainPanelOverride(object __instance)
        {
            try
            {
                var panelType = KlyteUtils.RunPrivateMethod<Type>(__instance, "GetDefaultClassForMainPanel");
                var textureAtlas = KlyteUtils.RunPrivateMethod<UITextureAtlas>(__instance, "GetTextureAtlasForIcon");
                var iconName = KlyteUtils.RunPrivateMethod<string>(__instance, "GetIconName");
                var tabWidth = KlyteUtils.RunPrivateMethod<int>(__instance, "GetTabWidth");
                var tooltipText = KlyteUtils.RunPrivateMethod<string>(__instance, "GetTooltipText");
                var enumValue = (ModTab)Enum.Parse(typeof(ModTab), KlyteUtils.RunPrivateMethod<string>(__instance, "GetEnumName"));
                KlyteModsPanel.instance.AddTab(enumValue, panelType, textureAtlas, iconName, tooltipText, (x, y) => { if (y) KlyteUtils.ExecuteReflectionMethod(__instance, "ShowVersionInfoPopup"); }, tabWidth);
                return false;

            }
            catch (Exception e)
            {
                KlyteUtils.doErrorLog($"{e.GetType()} detouring {__instance}: {e.Message}\n{e.StackTrace}");
            }
            return true;
        }
        #endregion

        #region Hooking

        public override void AwakeBody()
        {
            KlyteUtils.doLog("Loading NonKCModsOverrides");
            #region Suburb Styler
            foreach (var typeName in supportedClasses)
            {
                var type = Type.GetType(typeName);
                try
                {
                    if (type != null)
                    {
                        KlyteUtils.SetPrivateStaticField("GetMainReference", type, new Func<UIComponent>(() => KlyteModsPanel.instance.mainPanel));
                        KlyteUtils.SetPrivateStaticField("ClosePanel", type, new OnButtonClicked(() => KlyteCommonsMod.CloseKCPanel()));
                        KlyteUtils.SetPrivateStaticField("OpenPanel", type, new OnButtonClicked(() => KlyteModsPanel.instance.OpenAt((ModTab)Enum.Parse(typeof(ModTab), KlyteUtils.RunPrivateStaticMethod<string>(type, "GetEnumName")))));



                        MethodInfo createPanelOverride = typeof(NonKCModsOverrides).GetMethod("CreateMainPanelOverride", allFlags);

                        AddRedirect(type.GetMethod("CreateMainPanel", allFlags), createPanelOverride);
                    }
                }
                catch (Exception e)
                {
                    KlyteUtils.doErrorLog($"{e.GetType()} detouring {type}: {e.Message}\n{e.StackTrace}");
                }

            }
            #endregion
        }
        #endregion

        public override void doLog(string text, params object[] param)
        {
            KlyteUtils.doLog(text, param);
        }

    }
}
