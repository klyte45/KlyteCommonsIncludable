using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public abstract class KlyteResourceLoader<T> : Singleton<T> where T : KlyteResourceLoader<T>
    {
        protected abstract string prefix { get; }
        private Type resourceReference => typeof(T);

        public byte[] loadResourceData(string name)
        {
            name = prefix + name;

            UnmanagedMemoryStream stream = (UnmanagedMemoryStream)Assembly.GetAssembly(resourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                KlyteUtils.doErrorLog("Could not find resource: " + name);
                return null;
            }

            BinaryReader read = new BinaryReader(stream);
            return read.ReadBytes((int)stream.Length);
        }

        public string loadResourceString(string name)
        {
            name = prefix + name;

            UnmanagedMemoryStream stream = (UnmanagedMemoryStream)Assembly.GetAssembly(resourceReference).GetManifestResourceStream(name);
            if (stream == null)
            {
                KlyteUtils.doErrorLog("Could not find resource: " + name);
                return null;
            }

            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }

        public Texture2D loadTexture(int x, int y, string filename)
        {
            try
            {
                Texture2D texture = new Texture2D(x, y);
                texture.LoadImage(loadResourceData(filename));
                return texture;
            }
            catch (Exception e)
            {
                KlyteUtils.doErrorLog("The file could not be read:" + e.Message);
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
                tex.LoadImage(loadResourceData(textureFile));
                tex.Apply(true, true);
            }
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            { // Setup atlas
                Material material = (Material)Material.Instantiate(baseMaterial);
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

    public sealed class KCResourceLoader : KlyteResourceLoader<KCResourceLoader>
    {
        protected override string prefix => "Klyte.Commons.";
    }
}
