using ColossalFramework.UI;
using Klyte.Commons.Utils.UtilitiesClasses;
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
        public static UITextureAtlas DefaultTextureAtlas => UIView.GetAView().defaultAtlas;

        public static void LoadPathTexturesIntoInGameTextureAtlas(string path, ref List<SpriteInfo> newFiles) => LoadPathTexturesIntoTextureAtlas(UIView.GetAView().defaultAtlas, path, ref newFiles);
        public static void LoadPathTexturesIntoTextureAtlas(UITextureAtlas textureAtlas, string path, ref List<SpriteInfo> newFiles)
        {
            if (textureAtlas == null)
            {
                return;
            }

            string[] files = Directory.GetFiles(path, "*.png");
            if (files.Length == 0)
            {
                return;
            }
            var borderDescriptors = new Dictionary<string, Tuple<RectOffset, bool>>();
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
                        newFiles.AddRange(CreateSpriteInfo(borderDescriptors, filename, tex));
                    }
                }
            }
        }

        public static List<SpriteInfo> CreateSpriteInfo(Dictionary<string, Tuple<RectOffset, bool>> borderDescriptors, string filename, Texture2D tex)
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
            borderDescriptors.TryGetValue(generatedSpriteName, out Tuple<RectOffset, bool> border);
            var res = new SpriteInfo
            {
                texture = tex,
                name = generatedSpriteName,
                border = border?.First ?? new RectOffset()
            };
            if (border?.Second ?? false)
            {
                return new List<SpriteInfo>() {
                    res,
                    new SpriteInfo
                        {
                            texture = tex,
                            name = generatedSpriteName +NoBorderSuffix,
                            border =new RectOffset()
                        }
                    };
            }
            else
            {
                return new List<SpriteInfo>() { res };
            }
        }

        public static readonly string NoBorderSuffix = "_NOBORDER";

        public static void LoadImagesFromResources(string path, ref List<SpriteInfo> newSprites)
        {
            string[] imagesFiles = FileUtils.GetAllFilesEmbeddedAtFolder(path, ".png");
            TextureAtlasUtils.ParseBorderDescriptors(KlyteResourceLoader.LoadResourceStringLines($"{path}.{BORDER_FILENAME}"), out Dictionary<string, Tuple<RectOffset, bool>> borderDescriptor);
            foreach (string file in imagesFiles)
            {
                Texture2D tex = KlyteResourceLoader.LoadTexture($"{path}.{file}");
                if (tex != null)
                {
                    newSprites.AddRange(TextureAtlasUtils.CreateSpriteInfo(borderDescriptor, file, tex));
                }
            }
        }

        public static void ParseBorderDescriptors(IEnumerable<string> lines, out Dictionary<string, Tuple<RectOffset, bool>> borderDescriptors)
        {
            borderDescriptors = new Dictionary<string, Tuple<RectOffset, bool>>();
            foreach (string line in lines)
            {
                string[] lineSpilt = line.Split('=');
                if (lineSpilt.Length >= 2)
                {
                    string[] lineValues = lineSpilt[1].Split(',');
                    if (lineValues.Length == 4
                        && int.TryParse(lineValues[0], out int left)
                        && int.TryParse(lineValues[1], out int right)
                        && int.TryParse(lineValues[2], out int top)
                        && int.TryParse(lineValues[3], out int bottom)
                        )
                    {
                        borderDescriptors[lineSpilt[0]] = Tuple.New(new RectOffset(left, right, top, bottom), lineSpilt.Length >= 3 && bool.TryParse(lineSpilt[2], out bool noBorder) && noBorder);
                    }
                }
            }
        }

        public static void RegenerateDefaultTextureAtlas(List<SpriteInfo> newFiles) => RegenerateTextureAtlas(UIView.GetAView().defaultAtlas, newFiles);
        public static void RegenerateTextureAtlas(UITextureAtlas textureAtlas, List<SpriteInfo> newFiles)
        {
            IEnumerable<string> newSpritesNames = newFiles.Select(x => x.name);
            newFiles.AddRange(textureAtlas.sprites.Where(x => !newSpritesNames.Contains(x.name)));
            textureAtlas.sprites.Clear();
            textureAtlas.AddSprites(newFiles.ToArray());
            if (textureAtlas.texture == null)
            {
                textureAtlas.material.mainTexture = new Texture2D(1, 1);
                (textureAtlas.material.mainTexture as Texture2D).SetPixel(0, 0, default);
            }
            Rect[] array = textureAtlas.texture.PackTextures(textureAtlas.sprites.Select(x => x.texture).ToArray(), textureAtlas.padding, 4096 * 4);

            for (int i = 0; i < textureAtlas.count; i++)
            {
                textureAtlas.sprites[i].region = array[i];
            }
            textureAtlas.sprites.Sort();
            textureAtlas.RebuildIndexes();
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
