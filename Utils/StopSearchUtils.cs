using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class StopSearchUtils
    {
        #region Stop Search Utils
        public static List<ushort> FindNearStops(Vector3 position)
        {
            return FindNearStops(position, ItemClass.Service.PublicTransport, true, 24f, out _, out _);
        }
        public static List<ushort> FindNearStops(Vector3 position, ItemClass.Service service, bool allowUnderground, float maxDistance, out List<float> distanceSqrA, out List<Vector3> stopPositions, List<Quad2> boundaries = null)
        {
            return FindNearStops(position, service, service, VehicleInfo.VehicleType.None, allowUnderground, maxDistance, out distanceSqrA, out stopPositions, boundaries);
        }
        public static List<ushort> FindNearStops(Vector3 position, ItemClass.Service service, ItemClass.Service service2, VehicleInfo.VehicleType stopType, bool allowUnderground, float maxDistance,
             out List<float> distanceSqrA, out List<Vector3> stopPositions, List<Quad2> boundaries = null)
        {


            Bounds bounds = new Bounds(position, new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
            int num = Mathf.Max((int)((bounds.min.x - 64f) / 64f + 135f), 0);
            int num2 = Mathf.Max((int)((bounds.min.z - 64f) / 64f + 135f), 0);
            int num3 = Mathf.Min((int)((bounds.max.x + 64f) / 64f + 135f), 269);
            int num4 = Mathf.Min((int)((bounds.max.z + 64f) / 64f + 135f), 269);
            NetManager instance = Singleton<NetManager>.instance;
            List<Tuple<ushort, float, Vector3>> result = new List<Tuple<ushort, float, Vector3>>();

            float maxDistSqr = maxDistance * maxDistance;
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    var idx = i * 270 + j;
                    ushort nodeId = 0;
                    int num7 = 0;
                    try
                    {
                        nodeId = instance.m_nodeGrid[idx];
                        num7 = 0;
                        while (nodeId != 0)
                        {
                            NetInfo info = instance.m_nodes.m_buffer[(int)nodeId].Info;
                            if (info != null
                                && (info.m_class.m_service == service || info.m_class.m_service == service2)
                                && (instance.m_nodes.m_buffer[(int)nodeId].m_flags & (NetNode.Flags.Collapsed)) == NetNode.Flags.None
                                && (instance.m_nodes.m_buffer[(int)nodeId].m_flags & (NetNode.Flags.Created)) != NetNode.Flags.None
                                && instance.m_nodes.m_buffer[(int)nodeId].m_transportLine > 0
                                && (allowUnderground || !info.m_netAI.IsUnderground())
                                && (stopType == VehicleInfo.VehicleType.None || stopType == TransportManager.instance.m_lines.m_buffer[instance.m_nodes.m_buffer[nodeId].m_transportLine].Info.m_vehicleType))
                            {
                                var node = instance.m_nodes.m_buffer[(int)nodeId];
                                Vector3 nodePos = node.m_position;
                                if (boundaries != null && boundaries.Count != 0 && !boundaries.Any(x => x.Intersect(VectorUtils.XZ(nodePos))))
                                {
                                    goto GOTO_NEXT;
                                }
                                float delta = Mathf.Max(Mathf.Max(bounds.min.x - 64f - nodePos.x, bounds.min.z - 64f - nodePos.z), Mathf.Max(nodePos.x - bounds.max.x - 64f, nodePos.z - bounds.max.z - 64f));
                                if (delta < 0f && instance.m_nodes.m_buffer[(int)nodeId].m_bounds.Intersects(bounds))
                                {
                                    float num14 = Vector3.SqrMagnitude(position - nodePos);
                                    if (num14 < maxDistSqr)
                                    {
                                        result.Add(Tuple.New(nodeId, num14, nodePos));
                                    }
                                }
                            }
                            GOTO_NEXT:
                            nodeId = instance.m_nodes.m_buffer[(int)nodeId].m_nextGridNode;
                            if (++num7 >= 36864)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogUtils. DoErrorLog($"ERROR ON TRYING FindNearStops: (It = {num7}; Init = {idx}; Curr = {nodeId})==>  {e.Message}\n{e.StackTrace}");
                    }
                }
            }
            result = result.OrderBy(x => x.First).ToList();
            distanceSqrA = result.Select(x => x.Second).ToList();
            stopPositions = result.Select(x => x.Third).ToList();
            return result.Select(x => x.First).ToList();
        }


        private const float m_defaultStopOffset = 0.5019608f;

        public class StopPointDescriptorLanes
        {
            public Bezier3 platformLine;
            public float width;
            public VehicleInfo.VehicleType vehicleType;
        }

        public static StopPointDescriptorLanes[] MapStopPoints(BuildingInfo buildingInfo)
        {
            var result = new List<StopPointDescriptorLanes>();
            foreach (var path in buildingInfo.m_paths)
            {
                Vector3 position = path.m_nodes[0];
                Vector3 position2 = path.m_nodes[1];
                Vector3 directionPath = Quaternion.AngleAxis(90, Vector3.up) * (position2 - position).normalized;

                LogUtils.DoLog($"[{buildingInfo}] pos + dir = ({position} {position2} + {directionPath})");
                foreach (var lane in path.m_netInfo.m_lanes)
                {
                    if (lane.m_stopType == VehicleInfo.VehicleType.None) continue;
                    var lanePos = position + lane.m_position * directionPath;
                    var lanePos2 = position2 + lane.m_position * directionPath;
                    NetSegment.CalculateMiddlePoints(lanePos, Vector3.zero, lanePos2, Vector3.zero, true, true, out Vector3 b3, out Vector3 c);
                    var refBezier = new Bezier3(lanePos, b3, c, lanePos2);
                    LogUtils.DoLog($"[{buildingInfo}]refBezier = {refBezier} ({lanePos} {b3} {c} {lanePos2})");


                    var positionR = refBezier.Position(m_defaultStopOffset);
                    var direction = refBezier.Tangent(m_defaultStopOffset);
                    LogUtils.DoLog($"[{buildingInfo}]1positionR = {positionR}; direction = {direction}");

                    Vector3 normalized = Vector3.Cross(Vector3.up, direction).normalized;
                    positionR += normalized * (MathUtils.SmootherStep(0.5f, 0f, Mathf.Abs(m_defaultStopOffset - 0.5f)) * lane.m_stopOffset);
                    LogUtils.DoLog($"[{buildingInfo}]2positionR = {positionR}; direction = {direction}; {normalized}");
                    result.Add(new StopPointDescriptorLanes
                    {
                        platformLine = refBezier,
                        width = lane.m_width,
                        vehicleType = lane.m_stopType
                    });

                }
            }
            foreach (var subBuilding in buildingInfo.m_subBuildings)
            {
                var subPlats = MapStopPoints(subBuilding.m_buildingInfo);
                if (subPlats != null)
                {
                    result.AddRange(subPlats.Select(x =>
                    {
                        x.platformLine.a -= subBuilding.m_position;
                        x.platformLine.b -= subBuilding.m_position;
                        x.platformLine.c -= subBuilding.m_position;
                        x.platformLine.d -= subBuilding.m_position;
                        return x;
                    }));
                }
            }
            result.Sort((x, y) =>
            {
                var priorityX = VehicleToPriority(x.vehicleType);
                var priorityY = VehicleToPriority(y.vehicleType);
                if (priorityX != priorityY) return priorityX.CompareTo(priorityY);
                var centerX = x.platformLine.GetBounds().center;
                var centerY = y.platformLine.GetBounds().center;
                if (centerX.z != centerY.z) return centerX.z.CompareTo(centerY.z);
                if (centerX.x != centerY.x) return -centerX.x.CompareTo(centerY.x);
                return centerX.y.CompareTo(centerY.y);
            });
            return result.ToArray();
        }
        public static int VehicleToPriority(VehicleInfo.VehicleType tt)
        {
            switch (tt)
            {
                case VehicleInfo.VehicleType.Car:
                    return 99;
                case VehicleInfo.VehicleType.Metro:
                case VehicleInfo.VehicleType.Train:
                case VehicleInfo.VehicleType.Monorail:
                    return 20;
                case VehicleInfo.VehicleType.Ship:
                    return 10;
                case VehicleInfo.VehicleType.Plane:
                    return 5;
                case VehicleInfo.VehicleType.Tram:
                    return 88;
                case VehicleInfo.VehicleType.Helicopter:
                    return 7;
                case VehicleInfo.VehicleType.Ferry:
                    return 15;

                case VehicleInfo.VehicleType.CableCar:
                    return 30;
                case VehicleInfo.VehicleType.Blimp:
                    return 12;
                case VehicleInfo.VehicleType.Balloon:
                    return 11;
                default: return 9999;
            }
        }
        #endregion
    }
}
