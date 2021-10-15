using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using System;
using System.Collections.Generic;

namespace Klyte.Commons.Utils
{
    public class SortingUtils
    {
        #region Sorting

        public static int NaturalCompare(string left, string right) => (int)typeof(PublicTransportDetailPanel).GetMethod("NaturalCompare", RedirectorUtils.allFlags).Invoke(null, new object[] { left, right });

        public static void Quicksort<T>(IList<T> elements, Comparison<T> comp, bool invert) where T : UIComponent => Quicksort(elements, 0, elements.Count - 1, comp, invert);

        protected static void Quicksort<T>(IList<T> elements, int left, int right, Comparison<T> comp, bool invert) where T : UIComponent
        {
            int i = left;
            int num = right;
            T y = elements[(left + right) / 2];
            int multiplier = invert ? -1 : 1;
            while (i <= num)
            {
                while (comp(elements[i], y) * multiplier < 0)
                {
                    i++;
                }
                while (comp(elements[num], y) * multiplier > 0)
                {
                    num--;
                }
                if (i <= num)
                {
                    T value = elements[i];
                    elements[i] = elements[num];
                    elements[i].forceZOrder = i;
                    elements[num] = value;
                    elements[num].forceZOrder = num;
                    i++;
                    num--;
                }
            }
            if (left < num)
            {
                Quicksort(elements, left, num, comp, invert);
            }
            if (i < right)
            {
                Quicksort(elements, i, right, comp, invert);
            }
        }


        public static IList<Q> QuicksortList<Q>(IList<Q> elements, Comparison<Q> comp, bool invert) => QuicksortList(elements, 0, elements.Count - 1, comp, invert);
        protected static IList<Q> QuicksortList<Q>(IList<Q> elements, int left, int right, Comparison<Q> comp, bool invert)
        {
            int i = left;
            int num = right;
            var y = elements[(left + right) / 2];
            int multiplier = invert ? -1 : 1;
            while (i <= num)
            {
                while (comp(elements[i], y) * multiplier < 0)
                {
                    i++;
                }
                while (comp(elements[num], y) * multiplier > 0)
                {
                    num--;
                }
                if (i <= num)
                {
                    var value = elements[i];
                    elements[i] = elements[num];
                    elements[num] = value;
                    i++;
                    num--;
                }
            }
            if (left < num)
            {
                QuicksortList(elements, left, num, comp, invert);
            }
            if (i < right)
            {
                QuicksortList(elements, i, right, comp, invert);
            }
            return elements;
        }
        #endregion

    }
}
