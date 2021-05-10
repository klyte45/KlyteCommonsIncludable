using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using Klyte.Commons.Utils;
using System.Linq;

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
                    return x?.name == K45DialogControl.PANEL_ID && x.panelRoot?.gameObject != null && string.Compare(x.panelRoot?.stringUserData, K45DialogControl.VERSION) >= 0;
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

        public static void RemovePanel()
        {
            if(!(UIView.library is null))
            {
                UIView.library.m_DynamicPanels = UIView.library.m_DynamicPanels.Where(x => x?.name != K45DialogControl.PANEL_ID).ToArray();
            }
        }
    }
}