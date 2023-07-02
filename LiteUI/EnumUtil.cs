using Klyte.Commons.Utils;
using Klyte.Commons.Utils.StructExtensions;
using System;

namespace Klyte.Commons.LiteUI
{
    public static class EnumUtil
    {
        public static bool IsPow2(ulong x) => x != 0 && (x & (x - 1)) == 0;

        /// <summary>
        /// Converts any integer (including long) based enum to ulong .
        /// </summary>
        public static ulong ToUInt64(this Enum value)
        {
            Type enumType = value.GetType();
            bool signed = Enum.GetUnderlyingType(enumType).IsSignedInteger();
            return signed ? (ulong)(value as IConvertible).ToInt64(null) : (value as IConvertible).ToUInt64(null);
        }
    }
}
