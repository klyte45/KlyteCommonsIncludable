using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System.Collections.Generic;
using System.Linq;


namespace Klyte.Commons.UI
{
    public static class ColossalUIExtensions
    {
        public static Dictionary<T, int> GetReverseOptionsIndex<T>(this UIDropDown dd)
        {
            if (dd.objectUserData is Tuple<string, T>[] tupleArray)
            {
                return tupleArray.Select((x, i) => Tuple.New(x.Second, i)).ToDictionary(x => x.First, x => x.Second);
            }
            return new Dictionary<T, int>();
        }
    }
}
