namespace Klyte.Commons.Utils
{
    public static class StringExtensions
    {
        public static string Right(this string original, int numberCharacters) => original.Substring(original.Length - numberCharacters);
    }
}
