using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System;
using System.Collections;
using System.Reflection;

namespace Klyte.Commons.Overrides
{
    public class DistrictManagerOverrides : Redirector<DistrictManagerOverrides>
    {


        #region Events
        public delegate void OnDistrictChanged();
        public static event OnDistrictChanged eventOnDistrictRenamed;

        private static void OnDistrictRenamed()
        {
            DistrictManager.instance.StartCoroutine(RunEvent());
        }

        private static IEnumerator RunEvent()
        {
            yield return null;
            try
            {
                eventOnDistrictRenamed?.Invoke();
            }
            catch (Exception e)
            {
                KlyteUtils.doErrorLog(e.Message + "\n" + e.StackTrace);
            }
        }
        #endregion



        #region Hooking

        public override void AwakeBody()
        {
            KlyteUtils.doLog("Loading District Manager Overrides");
            #region Release Line Hooks
            MethodInfo posRename = typeof(DistrictManagerOverrides).GetMethod("OnDistrictRenamed", allFlags);

            AddRedirect(typeof(DistrictManager).GetMethod("UpdateNames", allFlags), null, posRename);
            #endregion


        }
        #endregion

        public override void doLog(string text, params object[] param)
        {
            KlyteUtils.doLog(text, param);
        }


    }
}
