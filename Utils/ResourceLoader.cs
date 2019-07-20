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
        private Type ResourceReference => typeof(T);
        public virtual Shader GetLoadedShader(string shaderName) => null;

        public byte[] LoadResourceData(string name)
        {
            name = Prefix + name;

            UnmanagedMemoryStream stream = (UnmanagedMemoryStream) Assembly.GetAssembly(ResourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                LogUtils.DoErrorLog("Could not find resource: " + name);
                return null;
            }

            BinaryReader read = new BinaryReader(stream);
            return read.ReadBytes((int) stream.Length);
        }

        public string LoadResourceString(string name)
        {
            name = Prefix + name;

            UnmanagedMemoryStream stream = (UnmanagedMemoryStream) Assembly.GetAssembly(ResourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                LogUtils.DoErrorLog("Could not find resource: " + name);
                return null;
            }

            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }

        public Texture2D LoadTexture(int x, int y, string filename)
        {
            try
            {
                Texture2D texture = new Texture2D(x, y);
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
            Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            { // LoadTexture
                tex.LoadImage(LoadResourceData(textureFile));
                tex.Apply(true, true);
            }
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            { // Setup atlas
                Material material = Material.Instantiate(baseMaterial);
                material.mainTexture = tex;
                atlas.material = material;
                atlas.name = atlasName;
            }
            // Add sprites
            for (int i = 0; i < spriteNames.Length; ++i)
            {
                float uw = 1.0f / spriteNames.Length;
                UITextureAtlas.SpriteInfo spriteInfo = new UITextureAtlas.SpriteInfo()
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
