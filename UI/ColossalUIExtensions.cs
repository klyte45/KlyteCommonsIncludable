using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Utils.UtilitiesClasses;
using System;
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

        public static Tuple<string, T>[] GetDropdownOptions<T>(string localeKey) where T : struct => Enum.GetValues(typeof(T)).Cast<T>().Select(x => Tuple.New(Locale.Get(localeKey, x.ToString()), x)).ToArray();
        public static Tuple<string, int>[] GetDropdownOptions(string[] localeKeys) => localeKeys.Select((x, i) => Tuple.New(Locale.Get(x), i)).ToArray();
        public static Tuple<string, int>[] GetDropdownOptionsUnlocalized(string[] values) => values.Select((x, i) => Tuple.New(x, i)).ToArray();
        public static Tuple<string, T>[] GetDropdownOptions<T>(this T[] options, string localeKey) where T : struct => options.Select(x => Tuple.New(Locale.Get(localeKey, x.ToString()), x)).ToArray();

        public static int GetCurrentIndexFor<T>(this UIDropDown dd, T value)
            => dd.objectUserData is Tuple<string, T>[] tupleArray
                ? tupleArray.Select((x, i) => Tuple.New(x.Second, i)).Where(x => Equals(x.First, value)).FirstOrDefault()?.Second ?? -1
                : -1;

        public static void SetSelection<T>(this UIDropDown dd, T value)
            => dd.selectedIndex = dd.GetCurrentIndexFor(value);


        public static T GetSelection<T>(this UIDropDown dd)
            => dd.objectUserData is Tuple<string, T>[] tupleArray && dd.selectedIndex >= 0
                ? tupleArray[dd.selectedIndex].Second
                : default;

    }
}
