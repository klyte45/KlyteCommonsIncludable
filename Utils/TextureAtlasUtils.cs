using ColossalFramework.UI;
using System;
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

        public static void LoadPathTexturesIntoInGameTextureAtlas(string path, ref List<SpriteInfo> newFiles)
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
                ParseBorderDescriptors(File.ReadAllLines($"{path}{Path.DirectorySeparatorChar}{BORDER_FILENAME}"), out borderDescriptors);
            }
            foreach (string filename in files)
            {
                if (File.Exists(filename))
                {
                    byte[] fileData = File.ReadAllBytes(filename);
                    var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(fileData))
                    {
                        newFiles.Add(CreateSpriteInfo(borderDescriptors, filename, tex));
                    }
                }
            }
        }

        public static SpriteInfo CreateSpriteInfo(Dictionary<string, RectOffset> borderDescriptors, string filename, Texture2D tex)
        {
            string textureName = Path.GetFileNameWithoutExtension(filename);
            string generatedSpriteName;
            if (textureName.StartsWith("%"))
            {
                generatedSpriteName = textureName.Substring(1);
            }
            else
            {
                generatedSpriteName = KlyteResourceLoader.GetDefaultSpriteNameFor(textureName);
            }
            borderDescriptors.TryGetValue(generatedSpriteName, out RectOffset border);
            var res = new SpriteInfo
            {
                texture = tex,
                name = generatedSpriteName,
                border = border ?? new RectOffset()
            };

            return res;
        }
        public static void LoadIamgesFromResources(string path, ref List<SpriteInfo> newSprites)
        {
            string[] imagesFiles = FileUtils.GetAllFilesEmbeddedAtFolder(path, ".png");
            TextureAtlasUtils.ParseBorderDescriptors(KlyteResourceLoader.LoadResourceStringLines($"{path}.{BORDER_FILENAME}"), out Dictionary<string, RectOffset> borderDescriptor);
            foreach (string file in imagesFiles)
            {
                Texture2D tex = KlyteResourceLoader.LoadTexture($"{path}.{file}");
                if (tex != null)
                {
                    newSprites.Add(TextureAtlasUtils.CreateSpriteInfo(borderDescriptor, file, tex));
                }
            }
        }

        public static void ParseBorderDescriptors(IEnumerable<string> lines, out Dictionary<string, RectOffset> borderDescriptors)
        {
            borderDescriptors = new Dictionary<string, RectOffset>();
            foreach (string line in lines)
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

        public static void RegenerateDefaultTextureAtlas(List<SpriteInfo> newFiles)
        {
            UITextureAtlas defaultTextureAtlas = UIView.GetAView().defaultAtlas;
            IEnumerable<string> newSpritesNames = newFiles.Select(x => x.name);
            newFiles.AddRange(defaultTextureAtlas.sprites.Where(x => !newSpritesNames.Contains(x.name)));
            defaultTextureAtlas.sprites.Clear();
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
        public static void ParseImageIntoDefaultTextureAtlas(Type enumType, string resourceName, int width, int height, ref List<SpriteInfo> sprites)
        {
            Array spriteValues = Enum.GetValues(enumType);
            Texture2D image = KlyteResourceLoader.LoadTexture(resourceName);
            for (int i = 0; i < spriteValues.Length && i * width < image.width; i++)
            {
                var textureQuad = new Texture2D(width, height, TextureFormat.RGBA32, false);
                textureQuad.SetPixels(image.GetPixels(i * width, 0, width, height));
                sprites.Add(new SpriteInfo()
                {
                    texture = textureQuad,
                    name = KlyteResourceLoader.GetDefaultSpriteNameFor(spriteValues.GetValue(i) as Enum)
                });
            }
        }
        public static void ParseImageIntoDefaultTextureAtlas<E>(string resourceName, int width, int height, ref List<SpriteInfo> sprites) where E : Enum => ParseImageIntoDefaultTextureAtlas(typeof(E), resourceName, width, height, ref sprites);
    }
}
