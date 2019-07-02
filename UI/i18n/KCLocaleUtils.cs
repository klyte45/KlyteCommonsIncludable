using ColossalFramework.Globalization;
using Klyte.Commons.Utils;
using System;

namespace Klyte.Commons.i18n
{
    public class KCLocaleUtils : KlyteLocaleUtils<KCLocaleUtils, KCResourceLoader>
    {
        public override string Prefix => "KCM_";

        protected override string PackagePrefix => "Klyte.Commons";

    }
}
