using ColossalFramework.Math;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public struct StopPointDescriptorLanes
    {
        public Bezier3 platformLine;
        public float width;
        public VehicleInfo.VehicleType vehicleType;
        public uint laneId;
        public sbyte subbuildingId;
        public Vector3 directionPath;
        public uint platformLaneId;

        public long UniquePlatformId => ((platformLaneId & 0x7FFFFFFF) << 31) | (laneId & 0x7FFFFFFF);

        public override string ToString() => $"{platformLine.Position(0.5f)} (w={width} | {vehicleType} | {subbuildingId} | {laneId} | DIR = {directionPath} ({directionPath.GetAngleXZ()}°))";
    }

}
