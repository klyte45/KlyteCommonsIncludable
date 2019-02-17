using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Utils;
using System;

namespace Klyte.Commons.i18n
{
    public abstract class KlyteLocaleUtils<T, R> : Singleton<T> where T : KlyteLocaleUtils<T, R> where R : KlyteResourceLoader<R>
    {
        private const string lineSeparator = "\r\n";
        private const string kvSeparator = "=";
        private const string idxSeparator = ">";
        private const string localeKeySeparator = "|";
        private const string commentChar = "#";
        private const string ignorePrefixChar = "%";
        private static string language = "";
        protected abstract string[] locales { get; }
        public abstract string prefix { get; }
        protected abstract string packagePrefix { get; }

        public string loadedLanguage
        {
            get {
                return language;
            }
        }

        public string loadedLanguageEffective
        {
            get {
                return language.Length == 0 ? "en" : language.Substring(0, 2);
            }
        }

        public string[] getLanguageIndex()
        {
            Array8<string> saida = new Array8<string>((uint)locales.Length + 1);
            saida.m_buffer[0] = Locale.Get(prefix + "GAME_DEFAULT_LANGUAGE");
            for (int i = 0; i < locales.Length; i++)
            {
                saida.m_buffer[i + 1] = Locale.Get(prefix + "LANG", locales[i]);
            }
            return saida.m_buffer;
        }

        public string getSelectedLocaleByIndex(int idx)
        {
            if (idx <= 0 || idx > locales.Length)
            {
                return "en";
            }
            return locales[idx - 1];
        }

        public void loadLocale(string localeId, bool force, string prefix = null, string packagePrefix = null)
        {
            if (force)
            {
                LocaleManager.ForceReload();
            }
            else
            {
                loadLocaleIntern(localeId, true, prefix ?? this.prefix, packagePrefix ?? this.packagePrefix);
            }
        }
        private void loadLocaleIntern(string localeId, bool setLocale, string prefix, string packagePrefix)
        {
            string load = Singleton<R>.instance.loadResourceString("UI.i18n." + localeId + ".properties");
            if (load == null)
            {
                KlyteUtils.doErrorLog("FILE " + "UI.i18n." + localeId + ".properties" + " NOT LOADED!!!!");
                load = Singleton<R>.instance.loadResourceString("UI.i18n.en.properties");
                if (load == null)
                {
                    KlyteUtils.doErrorLog("LOCALE NOT LOADED!!!!");
                    return;
                }
                localeId = "en";
            }
            var locale = KlyteUtils.GetPrivateField<Locale>(LocaleManager.instance, "m_Locale");
            Locale.Key k;


            foreach (var myString in load.Split(new string[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (myString.StartsWith(commentChar)) continue;
                if (!myString.Contains(kvSeparator)) continue;
                bool noPrefix = myString.StartsWith(ignorePrefixChar);
                var array = myString.Split(kvSeparator.ToCharArray(), 2);
                string value = array[1];
                int idx = 0;
                string localeKey = null;
                if (array[0].Contains(idxSeparator))
                {
                    var arrayIdx = array[0].Split(idxSeparator.ToCharArray());
                    if (!int.TryParse(arrayIdx[1], out idx))
                    {
                        continue;
                    }
                    array[0] = arrayIdx[0];

                }
                if (array[0].Contains(localeKeySeparator))
                {
                    array = array[0].Split(localeKeySeparator.ToCharArray());
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
                loadLocaleIntern("en", false, prefix, packagePrefix);
            }
            if (setLocale)
            {
                language = localeId;
            }

        }
    }
}
