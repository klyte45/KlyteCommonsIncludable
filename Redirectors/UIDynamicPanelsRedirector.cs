using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Redirectors
{

    public class UIDynamicPanelsRedirector : Redirector, IRedirectable
    {
        public Redirector RedirectorInstance => this;
        public void Awake()
        {
            System.Reflection.MethodInfo initMethod = typeof(UIDynamicPanels).GetMethod("Init", RedirectorUtils.allFlags);
            AddRedirect(initMethod, GetType().GetMethod("PreInit", RedirectorUtils.allFlags));

        }

        public static void PreInit(UIView view, UIDynamicPanels __instance)
        {
            int? ct = __instance.m_DynamicPanels?.Where(x =>
            {
                try
                {
                    return x?.name == K45DialogControl.PANEL_ID && x.panelRoot?.gameObject != null;
                }
                catch
                {
                    return false;
                }
            }).Count();
            if (ct == 0)
            {
                var listDynPanel = __instance.m_DynamicPanels.Where(x => x?.name != K45DialogControl.PANEL_ID).ToList();
                listDynPanel.Insert(0, K45DialogControl.CreatePanelInfo(view));
                __instance.m_DynamicPanels = listDynPanel.ToArray();
            }
        }
    }
}