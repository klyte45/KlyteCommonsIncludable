namespace Klyte.Commons.Utils
{
    public sealed class CommonResourceLoader : KlyteResourceLoader<CommonResourceLoader>
    {
        public override string Prefix => CommonProperties.ResourceBasePath;
    }
}
