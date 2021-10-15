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
        }      
    }
}
