using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System.Linq;

namespace Klyte.Commons.Redirectors
{

    public class UIDynamicPanelsRedirector : Redirector, IRedirectable
    {
        public Redirector RedirectorInstance => this;
        public void Awake() => AddRedirect(typeof(UIDynamicPanels).GetMethod("Init", RedirectorUtils.allFlags), GetType().GetMethod("PreInit", RedirectorUtils.allFlags));

        public static void PreInit(UIView view, UIDynamicPanels __instance)
        {

            LogUtils.DoWarnLog("PRE INIT!!!!");


            var listDynPanel = __instance.m_DynamicPanels.ToList();
            listDynPanel.Add(K45DialogControl.CreatePanelInfo(view));
            __instance.m_DynamicPanels = listDynPanel.ToArray();            
        }
    }
}