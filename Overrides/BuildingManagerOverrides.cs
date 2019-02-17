using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System.Reflection;

namespace Klyte.Commons.Overrides
{
    public class BuildingManagerOverrides : Redirector<DistrictManagerOverrides>
    {


        #region Events
        public delegate void OnBuildingNameChanged(ushort buildingID);
        public static event OnBuildingNameChanged eventOnBuildingRenamed;

        private static void OnBuildingRenamed(ushort building)
        {
            eventOnBuildingRenamed?.Invoke(building);
        }
        #endregion

        #region Hooking

        public override void AwakeBody()
        {
            KlyteUtils.doLog("Loading Building Manager Overrides");
            #region Release Line Hooks
            MethodInfo posRename = typeof(BuildingManagerOverrides).GetMethod("OnBuildingRenamed", allFlags);

            AddRedirect(typeof(BuildingManager).GetMethod("SetBuildingName", allFlags), null, posRename);
            #endregion


        }
        #endregion

        public override void doLog(string text, params object[] param)
        {
            KlyteUtils.doLog(text, param);
        }

        public static void CallBuildRenamedEvent(ushort building)
        {
            eventOnBuildingRenamed?.Invoke(building);
        }

    }
}
