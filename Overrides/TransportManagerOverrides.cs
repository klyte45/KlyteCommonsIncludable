using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System.Reflection;

namespace Klyte.Commons.Overrides
{
    public class TransportManagerOverrides : Redirector<TransportManagerOverrides>
    {


        #region Events
        public delegate void OnLineUpdated();
        public static event OnLineUpdated eventOnLineUpdated;

        private static void RunOnLineUpdated()
        {
            eventOnLineUpdated?.Invoke();
        }
        #endregion

        #region Hooking

        public override void AwakeBody()
        {
            KlyteUtils.doLog("Loading Transport Manager Overrides");
            #region Release Line Hooks
            MethodInfo posUpdate = typeof(TransportManagerOverrides).GetMethod("RunOnLineUpdated", allFlags);

            AddRedirect(typeof(TransportManager).GetMethod("UpdateLine", allFlags), null, posUpdate);
            #endregion


        }
        #endregion

        public override void doLog(string text, params object[] param)
        {
            KlyteUtils.doLog(text, param);
        }


    }
}
