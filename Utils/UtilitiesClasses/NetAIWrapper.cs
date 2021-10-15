using System.Reflection;

namespace Klyte.Commons.Utils
{
    public class NetAIWrapper
    {
        public NetAI AI { get; }
        private FieldInfo m_elevated;
        private FieldInfo m_bridge;
        private FieldInfo m_slope;
        private FieldInfo m_tunnel;
        private FieldInfo m_invisible;

        public NetAIWrapper(NetAI ai)
        {
            AI = ai;

            try
            {
                m_elevated = AI.GetType().GetField("m_elevatedInfo");
                m_bridge = AI.GetType().GetField("m_bridgeInfo");
                m_slope = AI.GetType().GetField("m_slopeInfo");
                m_tunnel = AI.GetType().GetField("m_tunnelInfo");
                m_invisible = AI.GetType().GetField("m_invisible");
            }
            catch
            {
                m_elevated = null;
                m_bridge = null;
                m_slope = null;
                m_tunnel = null;
                m_invisible = null;
            }
        }

        public bool HasElevation => m_elevated != null && m_bridge != null && m_slope != null && m_tunnel != null;

        public NetInfo Default
        {
            get => AI.m_info;
            set => AI.m_info = value;
        }

        public NetInfo Elevated
        {
            get => HasElevation ? m_elevated.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_elevated.SetValue(AI, value);
            }
        }

        public NetInfo Bridge
        {
            get => HasElevation ? m_bridge.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_bridge.SetValue(AI, value);
            }
        }

        public NetInfo Slope
        {
            get => HasElevation ? m_slope.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_slope.SetValue(AI, value);
            }
        }

        public NetInfo Tunnel
        {
            get => HasElevation ? m_tunnel.GetValue(AI) as NetInfo : null;
            set
            {
                if (!HasElevation)
                {
                    return;
                }

                m_tunnel.SetValue(AI, value);
            }
        }


        public bool IsInvisible() => m_invisible != null ? (bool)m_invisible.GetValue(AI) : false;


        public enum ElevationType
        {
            None = -1,
            Default,
            Tunnel,
            Ground,
            Elevated,
            Bridge,
            Slope
        }

        internal NetInfo RelativeTo(ElevationType elevationType)
        {
            if (!HasElevation)
            {
                return Default;
            }

            switch (elevationType)
            {
                default:
                case ElevationType.None:
                case ElevationType.Default:
                case ElevationType.Ground:
                    return Default;
                case ElevationType.Tunnel:
                    return Tunnel ?? Default;
                case ElevationType.Elevated:
                    return Elevated ?? Default;
                case ElevationType.Bridge:
                    return Bridge ?? Default;
                case ElevationType.Slope:
                    return Slope ?? Default;
            }
        }

        internal ElevationType ToType(NetInfo oldInfo)
        {
            if (!HasElevation)
            {
                return AI.IsUnderground() ? ElevationType.Tunnel : Default.m_clipTerrain ? ElevationType.Ground : ElevationType.Elevated;
            }

            if (oldInfo == Default)
            {
                return ElevationType.Ground;
            }
            if (oldInfo == Tunnel)
            {
                return ElevationType.Tunnel;
            }
            if (oldInfo == Elevated)
            {
                return ElevationType.Elevated;
            }
            if (oldInfo == Bridge)
            {
                return ElevationType.Bridge;
            }
            if (oldInfo == Slope)
            {
                return ElevationType.Slope;
            }
            return ElevationType.None;
        }
    }
}