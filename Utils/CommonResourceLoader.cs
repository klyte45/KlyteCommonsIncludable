namespace Klyte.Commons.Utils
{
    public sealed class CommonResourceLoader : KlyteResourceLoader<CommonResourceLoader>
    {
        public override string Prefix => CommonProperties.ResourceBasePath;

        public override string PrefixAtlasImage => "K45_CMM_";
    }
}
