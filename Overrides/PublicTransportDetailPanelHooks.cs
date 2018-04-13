using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Extensions;
using Klyte.Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.Commons.LineList
{
    class PublicTransportDetailPanelHooks : Redirector<PublicTransportDetailPanelHooks>
    {                

        #region Hooking

        public override void AwakeBody()
        {
            doLog("Loading PublicTransportLineInfo Hooks!");
            AddRedirect(typeof(PublicTransportLineInfo).GetMethod("Awake", allFlags), semiPreventDefaultMI);
            
        }
        public override void doLog(string text, params object[] param)
        {
            return;
        }


        #endregion
    }
    

}
