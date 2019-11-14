using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public abstract class KlyteResourceLoader<T> : Singleton<T> where T : KlyteResourceLoader<T>
    {
        public abstract string Prefix { get; }
        public abstract string PrefixAtlasImage { get; }
        private Type ResourceReference => typeof(T);
        public virtual Shader GetLoadedShader(string shaderName) => null;

        public string GetDefaultSpriteNameFor<E>(E value) where E : Enum
        {
            if (value.ToString().StartsWith("__"))
            {
                return value.ToString().Substring(2);
            }
            return $"{PrefixAtlasImage}_{value}";
        }

        public byte[] LoadResourceData(string name)
        {
            name = Prefix + name;

            var stream = (UnmanagedMemoryStream) Assembly.GetAssembly(ResourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                LogUtils.DoErrorLog("Could not find resource: " + name);
                return null;
            }

            var read = new BinaryReader(stream);
            return read.ReadBytes((int) stream.Length);
        }

        public string LoadResourceString(string name)
        {
            name = Prefix + name;

            var stream = (UnmanagedMemoryStream) Assembly.GetAssembly(ResourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                LogUtils.DoErrorLog("Could not find resource: " + name);
                return null;
            }

            var read = new StreamReader(stream);
            return read.ReadToEnd();
        }

        public Texture2D LoadTexture(string filename)
        {
            try
            {
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.LoadImage(LoadResourceData(filename));
                return texture;
            }
            catch (Exception e)
            {
                LogUtils.DoErrorLog("The file could not be read:" + e.Message);
            }

            return null;
        }

        public AssetBundle LoadBundle(string filename)
        {
            try
            {
                return AssetBundle.LoadFromMemory(LoadResourceData(filename));
            }
            catch (Exception e)
            {
                LogUtils.DoErrorLog("The file could not be read:" + e.Message);
            }

            return null;
        }

        public UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {
            var tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            { // LoadTexture
                tex.LoadImage(LoadResourceData(textureFile));
                tex.Apply(true, true);
            }
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            { // Setup atlas
                var material = Material.Instantiate(baseMaterial);
                material.mainTexture = tex;
                atlas.material = material;
                atlas.name = atlasName;
            }
            // Add sprites
            for (int i = 0; i < spriteNames.Length; ++i)
            {
                float uw = 1.0f / spriteNames.Length;
                var spriteInfo = new UITextureAtlas.SpriteInfo()
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };
                atlas.AddSprite(spriteInfo);
            }
            return atlas;
        }
    }
}
