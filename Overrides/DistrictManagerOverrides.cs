using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
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
            eventOnDistrictRenamed?.Invoke();
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
