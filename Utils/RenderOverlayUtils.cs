using ColossalFramework;
using ColossalFramework.Math;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons
{
    public static class RenderOverlayUtils
    {

        public static void RenderNetSegmentOverlay(RenderManager.CameraInfo cameraInfo, Color toolColor, ushort netSegment)
        {
            NetSegment[] segmentBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;
            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            if (netSegment == 0)
            {
                return;
            }
            NetInfo info = segmentBuffer[netSegment].Info;
            var startNode = segmentBuffer[netSegment].m_startNode;
            var endNode = segmentBuffer[netSegment].m_endNode;
            var smoothStart = (nodeBuffer[startNode].m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
            var smoothEnd = (nodeBuffer[endNode].m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
            Bezier3 bezier;
            bezier.a = nodeBuffer[startNode].m_position;
            bezier.d = nodeBuffer[endNode].m_position;
            NetSegment.CalculateMiddlePoints(bezier.a, segmentBuffer[netSegment].m_startDirection, bezier.d, segmentBuffer[netSegment].m_endDirection, smoothStart, smoothEnd, out bezier.b, out bezier.c);
            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, toolColor, bezier, info.m_halfWidth * 4f / 3f, 100000f, -100000f, -1f, 1280f, false, true);
            Segment3 segment;
            segment.a = nodeBuffer[startNode].m_position;
            Segment3 segment2;
            segment2.a = nodeBuffer[endNode].m_position;
            segment.b = GetControlPoint(netSegment);
            segment2.b = segment.b;
            toolColor.a /= 2f;
            Singleton<RenderManager>.instance.OverlayEffect.DrawSegment(cameraInfo, toolColor, segment, segment2, 0f, 10f, -1f, 1280f, false, true);
            Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, segment.b, info.m_halfWidth / 2f, -1f, 1280f, false, true);
        }

        private static Vector3 GetControlPoint(ushort segment)
        {
            NetSegment[] segmentBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;
            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            Vector3 position = nodeBuffer[segmentBuffer[segment].m_startNode].m_position;
            Vector3 startDirection = segmentBuffer[segment].m_startDirection;
            Vector3 position2 = nodeBuffer[segmentBuffer[segment].m_endNode].m_position;
            Vector3 endDirection = segmentBuffer[segment].m_endDirection;
            if (!NetSegment.IsStraight(position, startDirection, position2, endDirection, out _))
            {
                var num2 = (startDirection.x * endDirection.x) + (startDirection.z * endDirection.z);
                if (num2 >= -0.999f && Line2.Intersect(VectorUtils.XZ(position), VectorUtils.XZ(position + startDirection), VectorUtils.XZ(position2), VectorUtils.XZ(position2 + endDirection), out var d, out _))
                {
                    return position + (startDirection * d);
                }
                LogUtils.DoErrorLog("Warning! Invalid segment directions!");
            }
            return (position + position2) / 2f;
        }
    }
}
