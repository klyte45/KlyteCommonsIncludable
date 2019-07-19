using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System;

namespace Klyte.Commons.Interfaces
{
    public abstract class TextureAtlasDescriptor<A, R, E> : Singleton<A> where A : TextureAtlasDescriptor<A, R, E> where R : KlyteResourceLoader<R> where E : Enum
    {
        protected virtual int Width => 64;
        protected virtual int Height => 64;
        protected abstract string ResourceName { get; }
        protected abstract string CommonName { get; }

        protected UITextureAtlas m_atlas;

        public void Awake() => m_atlas = Singleton<R>.instance.CreateTextureAtlas(ResourceName, CommonName, (UIView.GetAView() ?? FindObjectOfType<UIView>()).defaultAtlas.material, Width, Height, Enum.GetNames(typeof(E)));

        public UITextureAtlas Atlas => m_atlas;
    }
}
