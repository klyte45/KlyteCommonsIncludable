using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.UI.i18n;
using Klyte.Commons.Utils;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Klyte.Commons.Redirectors
{
    [HarmonyPatch(typeof(UIDynamicPanels))]
    public static class UIDynamicPanelsRedirector
    {
        [HarmonyPatch(typeof(UIDynamicPanels), "Init")]
        [HarmonyPrefix]
        public static void PreInit(UIView view, UIDynamicPanels __instance)
        {
            int? ct = __instance.m_DynamicPanels?.Where(x =>
            {
                try
                {
                    return x?.name == K45DialogControl.PANEL_ID && x.panelRoot?.gameObject != null && string.Compare(x.panelRoot?.stringUserData, K45DialogControl.VERSION) >= 0;
                }
                catch
                {
                    return false;
                }
            }).Count();
            if (ct == 0)
            {
                var oldPanel = __instance.m_DynamicPanels.Where(x => x?.name == K45DialogControl.PANEL_ID).FirstOrDefault();
                if (oldPanel != null)
                {
                    LogUtils.DoWarnLog($"Unregistering older k45 panel (v: {oldPanel.panelRoot?.stringUserData})");
                    oldPanel.panelRoot.enabled = false;
                    oldPanel.Destroy();
                    Object.Destroy(oldPanel.panelRoot);
                }
                var listDynPanel = __instance.m_DynamicPanels.Where(x => x?.name != K45DialogControl.PANEL_ID).ToList();
                listDynPanel.Insert(0, K45DialogControl.CreatePanelInfo(view));
                __instance.m_DynamicPanels = listDynPanel.ToArray();
                KlyteLocaleManager.m_localeStringsDictionary(KlyteLocaleManager.m_localeManagerLocale(LocaleManager.instance))[new Locale.Key() { m_Identifier = KlyteLocaleManager.m_defaultTestKey }] = "OK";
                KlyteLocaleManager.m_localeStringsDictionary(KlyteLocaleManager.m_localeManagerLocale(LocaleManager.instance))[new Locale.Key() { m_Identifier = KlyteLocaleManager.m_defaultModControllingKey }] = CommonProperties.ModName;
            }
        }

        public static void RemovePanel()
        {
            if (!(UIView.library is null))
            {
                UIView.library.m_DynamicPanels = UIView.library.m_DynamicPanels.Where(x => x?.name != K45DialogControl.PANEL_ID).ToArray();
            }
        }
    }
}