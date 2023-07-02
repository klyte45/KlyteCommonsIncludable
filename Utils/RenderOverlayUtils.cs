using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace Klyte.Commons.Utils
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
            var startNode = segmentBuffer[netSegment].m_startNode;
            var endNode = segmentBuffer[netSegment].m_endNode;
            var smoothStart = (nodeBuffer[startNode].m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
            var smoothEnd = (nodeBuffer[endNode].m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
            Bezier3 bezier;
            bezier.a = nodeBuffer[startNode].m_position;
            bezier.d = nodeBuffer[endNode].m_position;
            NetSegment.CalculateMiddlePoints(bezier.a, segmentBuffer[netSegment].m_startDirection, bezier.d, segmentBuffer[netSegment].m_endDirection, smoothStart, smoothEnd, out bezier.b, out bezier.c);
            NetInfo info = segmentBuffer[netSegment].Info;
            bezier.RenderBezier(new RenderExtension.OverlayData(cameraInfo) { Width = info.m_halfWidth * 2F, Cut = true, Color = toolColor });
        }
    }
}
