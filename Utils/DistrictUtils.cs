using ColossalFramework;
using System.Collections.Generic;

namespace Klyte.Commons.Utils
{
    public class DistrictUtils
    {
        public static Dictionary<string, int> GetValidDistricts()
        {
            var districts = new Dictionary<string, int>
            {
                [$"<{Singleton<SimulationManager>.instance.m_metaData.m_CityName}>"] = 0
            };
            for (int i = 1; i <= 0x7F; i++)
            {
                if ((Singleton<DistrictManager>.instance.m_districts.m_buffer[i].m_flags & District.Flags.Created) != District.Flags.None)
                {
                    string districtName = Singleton<DistrictManager>.instance.GetDistrictName(i);
                    if (districts.ContainsKey(districtName))
                    {
                        districtName += $" (ID:{i})";
                    }
                    districts[districtName] = i;
                }
            }

            return districts;
        }
        public static Dictionary<string, int> GetValidParks()
        {
            var districts = new Dictionary<string, int>
            {
            };
            for (int i = 1; i <= 0x7F; i++)
            {
                if ((Singleton<DistrictManager>.instance.m_parks.m_buffer[i].m_flags & DistrictPark.Flags.Created) != DistrictPark.Flags.None)
                {
                    string districtName = Singleton<DistrictManager>.instance.GetParkName(i);
                    if (districts.ContainsKey(districtName))
                    {
                        districtName += $" (ID:{i})";
                    }
                    districts[districtName] = i;
                }
            }

            return districts;
        }
    }
}
