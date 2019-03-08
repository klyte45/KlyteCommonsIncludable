using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Overrides
{
    public class BuildingManagerOverrides : Redirector<BuildingManagerOverrides>
    {


        #region Events
        public delegate void OnBuildingNameChanged(ushort buildingID);
        public static event OnBuildingNameChanged eventOnBuildingRenamed;

        private static void OnBuildingRenamed(ushort building)
        {
            new AsyncAction(() =>
            {
                eventOnBuildingRenamed?.Invoke(building);
            }).Execute();
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
            BuildingManager.instance.StartCoroutine(CallBuildRenamedEvent_impl(building));
        }
        private static IEnumerator CallBuildRenamedEvent_impl(ushort building)
        {

            //returning 0 will make it wait 1 frame
            yield return new WaitForSeconds(1);


            //code goes here

            eventOnBuildingRenamed?.Invoke(building);
        }

    }
}
