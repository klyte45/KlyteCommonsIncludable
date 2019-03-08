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
        public static event Action eventOnDistrictChanged;

        private static void OnDistrictChanged()
        {
            new AsyncAction(() =>
            {
                DistrictManager.instance.StartCoroutine(RunEvent(eventOnDistrictChanged));
            }).Execute();
        }

        private static IEnumerator RunEvent(Action action)
        {
            yield return null;
            yield return null;
            try
            {
                action?.Invoke();
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
            MethodInfo posChange = typeof(DistrictManagerOverrides).GetMethod("OnDistrictChanged", allFlags);

            AddRedirect(typeof(DistrictManager).GetMethod("SetDistrictName", allFlags), null, posChange);
            AddRedirect(typeof(DistrictManager).GetMethod("AreaModified", allFlags), null, posChange);
            #endregion


        }
        #endregion

        public override void doLog(string text, params object[] param)
        {
            KlyteUtils.doLog(text, param);
        }


    }
}
