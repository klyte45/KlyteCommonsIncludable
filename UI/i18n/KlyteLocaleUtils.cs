using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Utils;
using System;

namespace Klyte.Commons.i18n
{
    public abstract class KlyteLocaleUtils<T, R> : Singleton<T> where T : KlyteLocaleUtils<T, R> where R : KlyteResourceLoader<R>
    {

        internal static SavedInt CurrentLanguageId => new SavedInt("KlyteCommonsLanguage", Settings.gameSettingsFile, 0, true);

        private const string m_lineSeparator = "\r\n";
        private const string m_kvSeparator = "=";
        private const string m_idxSeparator = ">";
        private const string m_localeKeySeparator = "|";
        private const string m_commentChar = "#";
        private const string m_ignorePrefixChar = "%";
        private static string m_language = "";
        protected readonly string[] locales = new string[] { "en", "pt", "ko", "de", "cn", "pl", "nl", "fr", "es", "ru" };
        public abstract string Prefix { get; }
        protected abstract string PackagePrefix { get; }

        public string LoadedLanguage
        {
            get {
                return m_language;
            }
        }

        public string LoadedLanguageEffective
        {
            get {
                return m_language.Length == 0 ? "en" : m_language.Substring(0, 2);
            }
        }

        public string[] GetLanguageIndex()
        {
            Array8<string> saida = new Array8<string>((uint)locales.Length + 1);
            saida.m_buffer[0] = Locale.Get(Prefix + "GAME_DEFAULT_LANGUAGE");
            for (int i = 0; i < locales.Length; i++)
            {
                saida.m_buffer[i + 1] = Locale.Get(Prefix + "LANG", locales[i]);
            }
            return saida.m_buffer;
        }

        public string GetSelectedLocaleByIndex()
        {
            var idx = CurrentLanguageId.value;
            if (idx == 0)
            {
                return SingletonLite<LocaleManager>.instance.language;
            }
            if (idx < 0 || idx > locales.Length)
            {
                return "en";
            }
            return locales[idx - 1];
        }

        public void LoadCurrentLocale(bool force, string prefix = null, string packagePrefix = null)
        {
            LoadLocale(GetSelectedLocaleByIndex(), force, prefix ?? this.Prefix, packagePrefix ?? this.PackagePrefix);
        }

        internal virtual void LoadLocale(string localeId, bool force, string prefix = null, string packagePrefix = null)
        {
            LoadLocaleIntern(localeId, true, prefix ?? this.Prefix, packagePrefix ?? this.PackagePrefix);
        }
        private void LoadLocaleIntern(string localeId, bool setLocale, string prefix, string packagePrefix)
        {
            LogUtils.DoLog($"{GetType()} localeId: {localeId}");
            string load = Singleton<R>.instance.LoadResourceString("UI.i18n." + localeId + ".properties");
            if (load == null)
            {
                LogUtils.DoLog("File UI.i18n." + localeId + ".properties not found. Probably this translation doesn't exists for this mod.");
                load = "";
            }
            var locale = ReflectionUtils.GetPrivateField<Locale>(LocaleManager.instance, "m_Locale");
            Locale.Key k;


            foreach (var myString in load.Split(m_lineSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (myString.StartsWith(m_commentChar)) continue;
                if (!myString.Contains(m_kvSeparator)) continue;
                bool noPrefix = myString.StartsWith(m_ignorePrefixChar);
                var array = myString.Split(m_kvSeparator.ToCharArray(), 2);
                string value = array[1];
                int idx = 0;
                string localeKey = null;
                if (array[0].Contains(m_idxSeparator))
                {
                    var arrayIdx = array[0].Split(m_idxSeparator.ToCharArray());
                    if (!int.TryParse(arrayIdx[1], out idx))
                    {
                        continue;
                    }
                    array[0] = arrayIdx[0];

                }
                if (array[0].Contains(m_localeKeySeparator))
                {
                    array = array[0].Split(m_localeKeySeparator.ToCharArray());
                    localeKey = array[1];
                }

                k = new Locale.Key()
                {
                    m_Identifier = noPrefix ? array[0].Substring(1) : prefix + array[0],
                    m_Key = localeKey,
                    m_Index = idx
                };
                if (!locale.Exists(k))
                {
                    locale.AddLocalizedString(k, value.Replace("\\n", "\n"));
                }
            }

            if (localeId != "en")
            {
                LoadLocaleIntern("en", false, prefix, packagePrefix);
            }
            if (setLocale)
            {
                m_language = localeId;
            }

        }
    }
}
