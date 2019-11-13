using ColossalFramework.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static ColossalFramework.UI.UITextureAtlas;

namespace Klyte.Commons.Utils
{
    public static class TextureAtlasUtils
    {
        public static void LoadPathTexturesIntoInGameTextureAtlas(string prefix, string path)
        {
            UITextureAtlas defaultTextureAtlas = UIView.GetAView().defaultAtlas;
            if (defaultTextureAtlas == null)
            {
                return;
            }

            string[] files = Directory.GetFiles(path, "*.png");
            if (files.Length == 0)
            {
                return;
            }
            var newFiles = new List<SpriteInfo>();
            foreach (string filename in files)
            {
                if (File.Exists(filename))
                {
                    byte[] fileData = File.ReadAllBytes(filename);
                    var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(fileData))
                    {
                        string textureName = Path.GetFileNameWithoutExtension(filename);
                        string generatedSpriteName = $"K45_{prefix}_{Path.GetFileNameWithoutExtension(filename)}";
                        if (textureName.StartsWith("%"))
                        {
                            generatedSpriteName = textureName.Substring(1);
                        }
                        newFiles.Add(new SpriteInfo
                        {
                            texture = tex,
                            name = generatedSpriteName,
                            border = new RectOffset()
                        });
                    }
                }
            }
            if (newFiles.Count > 0)
            {
                defaultTextureAtlas.AddSprites(newFiles.ToArray());
                Rect[] array = defaultTextureAtlas.texture.PackTextures(defaultTextureAtlas.sprites.Select(x => x.texture).ToArray(), defaultTextureAtlas.padding, 4096 * 4);
                for (int i = 0; i < defaultTextureAtlas.count; i++)
                {
                    defaultTextureAtlas.sprites[i].region = array[i];
                }
                defaultTextureAtlas.sprites.Sort();
                defaultTextureAtlas.RebuildIndexes();
                UIView.RefreshAll(false);
            }
        }
    }
}
