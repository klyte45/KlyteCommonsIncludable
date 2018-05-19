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
