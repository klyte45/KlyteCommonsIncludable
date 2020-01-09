using ColossalFramework.Globalization;
using ColossalFramework.Math;
using Klyte.Commons.Extensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klyte.Commons.Utils
{
    public class VehicleUtils
    {
        #region Vehicle Utils
        public static VehicleInfo GetRandomModel(IEnumerable<string> assetList, out string selectedModel)
        {
            selectedModel = null;
            if (assetList.Count() == 0)
            {
                return null;
            }

            var r = new Randomizer(new System.Random().Next());

            selectedModel = assetList.ElementAt(r.Int32(0, assetList.Count() - 1));

            VehicleInfo saida = PrefabCollection<VehicleInfo>.FindLoaded(selectedModel);
            if (saida == null)
            {
                LogUtils.DoLog("MODEL DOESN'T EXIST!");
                return null;
            }
            return saida;
        }
        public static int GetCapacity(VehicleInfo info) => GetCapacity(info, info.m_vehicleAI);
        public static int GetCapacity<AI>(VehicleInfo info, AI ai, bool noLoop = false) where AI : VehicleAI
        {
            if (info == null)
            {
                return -1;
            }
            System.Reflection.FieldInfo fieldInfo = ai.GetType().GetField("m_passengerCapacity", RedirectorUtils.allFlags);
            if (fieldInfo != null)
            {
                int capacity = ReflectionUtils.GetGetFieldDelegate<AI, int>(fieldInfo)(ai);
                try
                {
                    if (!noLoop)
                    {
                        foreach (VehicleInfo.VehicleTrailer trailer in info.m_trailers)
                        {
                            capacity += trailer.m_info == null ? 0 : GetCapacity(trailer.m_info, trailer.m_info.m_vehicleAI, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtils.DoLog($"ERRO AO OBTER CAPACIDADE: [{info}] {e} {e.Message}\n{e.StackTrace}");
                }
                return capacity;
            }
            else
            {
                LogUtils.DoLog($"AI \"{ai.GetType()}\" in asset {info} has no passenger Field!");
                return 0;
            }
        }
        public static Dictionary<string, float> GetCapacityRelative(VehicleInfo info)
        {
            var relativeParts = new Dictionary<string, float>();
            GetCapacityRelative(info, info.m_vehicleAI, ref relativeParts, out _);
            return relativeParts;
        }

        private static void GetCapacityRelative<AI>(VehicleInfo info, AI ai, ref Dictionary<string, float> relativeParts, out int totalCapacity, bool noLoop = false) where AI : VehicleAI
        {
            if (info == null)
            {
                totalCapacity = 0;
                return;
            }

            totalCapacity = ReflectionUtils.GetGetFieldDelegate<AI, int>("m_passengerCapacity", ai.GetType())(ai);
            relativeParts[info.name] = totalCapacity;
            if (!noLoop)
            {
                try
                {
                    foreach (VehicleInfo.VehicleTrailer trailer in info.m_trailers)
                    {
                        if (trailer.m_info != null)
                        {
                            GetCapacityRelative(trailer.m_info, trailer.m_info.m_vehicleAI, ref relativeParts, out int capacity, true);
                            totalCapacity += capacity;
                        }
                    }

                    for (int i = 0; i < relativeParts.Keys.Count; i++)
                    {
                        relativeParts[relativeParts.Keys.ElementAt(i)] /= totalCapacity;
                    }
                }
                catch (Exception e)
                {
                    LogUtils.DoLog($"ERRO AO OBTER CAPACIDADE REL: [{info}] {e} {e.Message}\n{e.StackTrace}");
                }
            }
        }

        public static bool IsTrailer(PrefabInfo prefab)
        {
            string @unchecked = Locale.GetUnchecked("VEHICLE_TITLE", prefab.name);
            return @unchecked.StartsWith("VEHICLE_TITLE") || @unchecked.StartsWith("Trailer");
        }

        public static void ReplaceVehicleModel(ushort idx, VehicleInfo newInfo)
        {
            VehicleManager instance = VehicleManager.instance;
            CitizenManager.instance.ReleaseUnits(instance.m_vehicles.m_buffer[idx].m_citizenUnits);
            instance.m_vehicles.m_buffer[idx].Unspawn(idx);
            instance.m_vehicles.m_buffer[idx].Info = newInfo;
            instance.m_vehicles.m_buffer[idx].Spawn(idx);
            newInfo.m_vehicleAI.CreateVehicle(idx, ref instance.m_vehicles.m_buffer[idx]);
        }

        #endregion
    }

}