using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.Commons.Overrides
{
    public class DistrictManagerOverrides : Redirector<DistrictManagerOverrides>
    {


        #region Events
        public delegate void OnDistrictChanged();
        public static event OnDistrictChanged eventOnDistrictRenamed;

        private static bool OnDistrictRenamed()
        {
            eventOnDistrictRenamed?.Invoke();
            return true;
        }
        #endregion

        #region Hooking

        public override void AwakeBody()
        {
            KlyteUtils.doLog("Loading District Manager Overrides");
            #region Release Line Hooks
            MethodInfo preRename = typeof(DistrictManagerOverrides).GetMethod("OnDistrictRenamed", allFlags);

            AddRedirect(typeof(DistrictManager).GetMethod("UpdateNames", allFlags), preRename);
            #endregion


        }
        #endregion

        public override void doLog(string text, params object[] param)
        {

        }


    }
}
