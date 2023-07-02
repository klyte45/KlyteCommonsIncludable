using ColossalFramework;

namespace Klyte.Commons.Utils.StructExtensions
{
    public static class StringExtensions
    {
        public static string Right(this string original, int numberCharacters) => original.Substring(original.Length - numberCharacters);
        public static string TrimToNull(this string original) => original.IsNullOrWhiteSpace() ? null : original.Trim();
    }
}
