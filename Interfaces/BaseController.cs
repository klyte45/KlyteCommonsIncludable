using Klyte.Commons.ModShared;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public class BaseController<U, C> : MonoBehaviour
        where U : BasicIUserModSimplified<U, C>, new()
        where C : BaseController<U, C>
    {
        public void Start() => StartActions();

        private IBridgeUUI m_bridgeUUI;
        internal IBridgeUUI BridgeUUI
        {
            get
            {
                if (m_bridgeUUI is null)
                {
                    m_bridgeUUI = BasicIUserModSimplified<U, C>.UseUuiIfAvailable
                        ? PluginUtils.GetImplementationTypeForMod<BridgeUUIFallback, IBridgeUUI>(gameObject, "UnifiedUILib", "2.1.12", "Klyte.Commons.ModShared.BridgeUUI")
                        : gameObject.AddComponent<BridgeUUIFallback>();
                }
                return m_bridgeUUI;
            }
        }

        protected virtual void StartActions() { }
    }
}