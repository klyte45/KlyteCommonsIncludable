

using Klyte.Commons.Utils;
using System;
using UnityEngine;

namespace Klyte.Addresses.Shared
{
    public static class AdrEvents
    {
        public static event Action eventZeroMarkerBuildingChange;

        public static void TriggerZeroMarkerBuildingChange()
        {
            eventZeroMarkerBuildingChange?.Invoke();
        }
    }
}