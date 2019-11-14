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
        public static string BORDER_FILENAME = "bordersDescriptor.txt";

        public static void LoadPathTexturesIntoInGameTextureAtlas(string prefix, string path, ref List<SpriteInfo> newFiles)
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
            var borderDescriptors = new Dictionary<string, RectOffset>();
            if (File.Exists($"{path}{Path.DirectorySeparatorChar}{BORDER_FILENAME}"))
            {
                foreach (string line in File.ReadAllLines($"{path}{Path.DirectorySeparatorChar}{BORDER_FILENAME}"))
                {
                    string[] lineSpilt = line.Split('=');
                    if (lineSpilt.Length == 2)
                    {
                        string[] lineValues = lineSpilt[1].Split(',');
                        if (lineValues.Length == 4
                            && int.TryParse(lineValues[0], out int left)
                            && int.TryParse(lineValues[1], out int right)
                            && int.TryParse(lineValues[2], out int top)
                            && int.TryParse(lineValues[3], out int bottom)
                            )
                        {
                            borderDescriptors[lineSpilt[0]] = new RectOffset(left, right, top, bottom);
                        }
                    }
                }
            }
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
                            textureName = textureName.Substring(1);
                            generatedSpriteName = textureName;
                        }
                        borderDescriptors.TryGetValue(generatedSpriteName, out RectOffset border);
                        newFiles.Add(new SpriteInfo
                        {
                            texture = tex,
                            name = generatedSpriteName,
                            border = border ?? new RectOffset()
                        });
                    }
                }
            }
        }

        public static void RegenerateDefaultTextureAtlas(List<SpriteInfo> newFiles)
        {
            UITextureAtlas defaultTextureAtlas = UIView.GetAView().defaultAtlas;
            IEnumerable<string> newSpritesNames = newFiles.Select(x => x.name);
            defaultTextureAtlas.sprites.RemoveAll(x => newSpritesNames.Contains(x.name));
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
