using ColossalFramework.Globalization;
using Klyte.Commons.Utils;
using System;

namespace Klyte.Commons.i18n
{
    public class KCLocaleUtils : KlyteLocaleUtils<KCLocaleUtils, KCResourceLoader>
    {
        protected override string[] locales => new string[] { "en", "pt" };

        public override string prefix => "KCM_";

        protected override string packagePrefix => "Klyte.Commons";
    }
}
