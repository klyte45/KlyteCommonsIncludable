using System;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class SerializationUtils
    {
        #region Default (de)serialization
        public static string SerializeColor(Color32 value, string separator)
        {
            return string.Join(separator, new string[] { value.r.ToString(), value.g.ToString(), value.b.ToString() });
        }

        public static Color DeserializeColor(string value, string separator)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var list = value.Split(separator.ToCharArray()).ToList();
                if (list.Count == 3 && byte.TryParse(list[0], out byte r) && byte.TryParse(list[1], out byte g) && byte.TryParse(list[2], out byte b))
                {
                    return new Color32(r, g, b, 255);
                }
                else
                {
                    LogUtils.DoLog($"val = {value}; list = {String.Join(",", list.ToArray())} (Size {list.Count})");
                }
            }
            return Color.clear;
        }
        #endregion
    }
}
