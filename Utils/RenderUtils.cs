using ColossalFramework;
using ColossalFramework.UI;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class TextureRenderUtils
    {
        public static Texture2D RenderSpriteLineToTexture(UIDynamicFont font, UITextureAtlas atlas, string spriteName, Color bgColor, string text) => RenderSpriteLine(font, atlas, spriteName, bgColor, text);
        public static Texture2D RenderTextToTexture(UIDynamicFont font, string text, Color textColor, out Vector2 textDimensions, Color outlineColor = default)
        {
            float textScale = 2f;

            textDimensions = MeasureTextWidth(font, text, textScale, out Vector2 yBounds);


            var tex = new Texture2D((int) textDimensions.x, (int) textDimensions.y, TextureFormat.ARGB32, false);
            tex.SetPixels(new Color[(int) (textDimensions.x * textDimensions.y)]);

            RenderText(font, text, new Vector3(0, -yBounds.x), textScale, textColor, outlineColor, tex);

            var imageSize = new Vector2(Mathf.Max(Mathf.NextPowerOfTwo((int) textDimensions.x), 1), Mathf.Max(Mathf.NextPowerOfTwo((int) textDimensions.y), 1));

            TextureScaler.scale(tex, (int) imageSize.x, (int) imageSize.y);
            return tex;

        }
        public static Texture2D RenderSpriteLine(UIDynamicFont font, UITextureAtlas atlas, string spriteName, Color bgColor, string text)
        {

            UITextureAtlas.SpriteInfo spriteInfo = atlas[spriteName];
            if (spriteInfo == null)
            {
                CODebugBase<InternalLogChannel>.Warn(InternalLogChannel.UI, "Missing sprite " + spriteName + " in " + atlas.name);
                return null;
            }
            else
            {
                float textScale = 2f;

                Texture2D texture = atlas.texture;
                float calcHeight = font.size * textScale * 2;
                float calcProportion = spriteInfo.region.width * texture.width / (spriteInfo.region.height * texture.height);
                float calcWidth = Mathf.CeilToInt(calcHeight * calcProportion);

                int height = Mathf.CeilToInt(calcHeight);
                int width = Mathf.CeilToInt(calcWidth);

                float textureScale = height / (spriteInfo.region.height * texture.height);

                LogUtils.DoLog($"height = {height} - width = {width} -  renderer.pixelRatio = 1 - textureScale = {height} / {(spriteInfo.region.height * texture.height)}");

                var size = new Vector3(width, height);

                Vector2 textDimensions = MeasureTextWidth(font, text, textScale, out Vector2 yBounds);
                float multipler = Mathf.Min(Mathf.Min(3.5f, size.x / textDimensions.x), Mathf.Min(3.5f, size.y / textDimensions.y));
                if (multipler > 1)
                {
                    textScale *= 1 + ((multipler - 1) / 2.1f);
                    multipler = 1;
                    textDimensions = MeasureTextWidth(font, text, textScale, out yBounds);
                }
                float midLineOffset = (font).baseline / 2 * textScale;

                var imageSize = new Vector2(Mathf.NextPowerOfTwo((int) Mathf.Max(textDimensions.x * multipler, width)), Mathf.NextPowerOfTwo((int) Mathf.Max(textDimensions.y, height)));


                var tex = new Texture2D((int) imageSize.x, (int) imageSize.y, TextureFormat.ARGB32, false);
                tex.SetPixels(new Color[(int) (imageSize.x * imageSize.y)]);


                var texText = new Texture2D((int) textDimensions.x, (int) textDimensions.y, TextureFormat.ARGB32, false);
                texText.SetPixels(new Color[(int) (textDimensions.x * textDimensions.y)]);


                Vector2 position = RenderSprite(atlas, spriteName, bgColor, tex, textureScale);
                Vector2 posText = position + new Vector2((size.x / 2) - (textDimensions.x * multipler / 2) + 1, (size.y / 2) - (textDimensions.y / 2) - (yBounds.x / 2));

                RenderText(font, text, new Vector3(0, -yBounds.x), textScale, KlyteMonoUtils.ContrastColor(bgColor), bgColor, texText);

                if (multipler < 1)
                {
                    TextureScaler.scale(texText, (int) (texText.width * multipler), texText.height);
                }
                MergeTextures(tex, texText.GetPixels(), (int) posText.x, (int) posText.y, texText.width, texText.height, false);
                Object.Destroy(texText);
                tex.Apply();

                return tex;
            }
        }
        private static Vector2 MeasureTextWidth(UIDynamicFont font, string text, float textScale, out Vector2 yBounds)
        {
            float width = 1f;
            int size = Mathf.CeilToInt(font.size * textScale);
            font.RequestCharacters(text, size, FontStyle.Normal);
            yBounds = new Vector2(9999999f, -999999999f);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                font.baseFont.GetCharacterInfo(c, out CharacterInfo characterInfo, size, FontStyle.Normal);
                if (c == '\t')
                {
                    width += 3f * characterSpacing;
                }
                else
                {
                    width += ((c != ' ') ? characterInfo.maxX : (characterInfo.advance + (characterSpacing * textScale)));
                    yBounds.x = Mathf.Min(yBounds.x, characterInfo.minY);
                    yBounds.y = Mathf.Max(yBounds.y, characterInfo.maxY);
                }
            }
            if (text.Length > 2)
            {
                width += (text.Length - 2) * characterSpacing * textScale;
            }
            return new Vector2(width + 6, yBounds.y - yBounds.x + 6);
        }
        private static void RenderText(UIDynamicFont uidynamicFont, string text, Vector3 position, float textScale, Color textColor, Color outlineColor, Texture2D tex)
        {
            float size = (uidynamicFont.size * textScale);
            FontStyle style = FontStyle.Normal;
            float x = position.x + 3;
            float y = position.y + 3;
            Color color2 = textColor;
            Color c = color2;
            Texture2D readableTex = ((Texture2D) uidynamicFont.baseFont.material.mainTexture).MakeReadable();
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0)
                {
                    x += characterSpacing * textScale;
                }
                if (uidynamicFont.baseFont.GetCharacterInfo(text[i], out CharacterInfo glyph, Mathf.CeilToInt(size), style))
                {
                    if (text[i] == ' ')
                    {
                        x += (glyph.advance + (characterSpacing * textScale));
                        continue;
                    }
                    float num3 = (glyph.maxY);
                    float minX = x + glyph.minX;
                    float maxY = y + num3;
                    float minY = maxY - glyph.glyphHeight;
                    var vector4 = new Vector3(minX, minY);

                    float minU = Mathf.Min(glyph.uvTopLeft.x, glyph.uvTopRight.x, glyph.uvBottomRight.x, glyph.uvBottomLeft.x);
                    float maxU = Mathf.Max(glyph.uvTopLeft.x, glyph.uvTopRight.x, glyph.uvBottomRight.x, glyph.uvBottomLeft.x);
                    float minV = Mathf.Min(glyph.uvTopLeft.y, glyph.uvTopRight.y, glyph.uvBottomRight.y, glyph.uvBottomLeft.y);
                    float maxV = Mathf.Max(glyph.uvTopLeft.y, glyph.uvTopRight.y, glyph.uvBottomRight.y, glyph.uvBottomLeft.y);
                    int sizeU = (int) ((maxU - minU) * readableTex.width);
                    int sizeV = (int) ((maxV - minV) * readableTex.height);
                    Color[] colors = readableTex.GetPixels(Mathf.RoundToInt(minU * readableTex.width), Mathf.RoundToInt(minV * readableTex.height), sizeU, sizeV);
                    if (outlineColor != default)
                    {
                        for (int j = 0; j < kOutlineOffsets.Length; j++)
                        {
                            Vector3 b2 = kOutlineOffsets[j] * 3;
                            Vector3 targetOffset = vector4 + b2;
                            MergeTextures(tex, colors.Select(x => x.a * outlineColor).ToArray(), Mathf.RoundToInt(targetOffset.x), Mathf.RoundToInt(targetOffset.y), glyph.glyphWidth, glyph.glyphHeight, glyph.flipped, !glyph.flipped, glyph.flipped, true);
                        }
                    }


                    MergeTextures(tex, colors.Select(x => x.a * textColor).ToArray(), Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y), glyph.glyphWidth, glyph.glyphHeight, glyph.flipped, !glyph.flipped, glyph.flipped);
                    x += glyph.maxX;
                }
            }
        }

        private static void MergeTextures(Texture2D tex, Color[] colors, int startX, int startY, int sizeX, int sizeY, bool swapXY = false, bool flipVertical = false, bool flipHorizontal = false, bool plain = false)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Color orPixel = tex.GetPixel(startX + i, startY + j);
                    Color newPixel = colors[((flipVertical ? sizeY - j - 1 : j) * (swapXY ? 1 : sizeX)) + ((flipHorizontal ? sizeX - i - 1 : i) * (swapXY ? sizeY : 1))];

                    if (plain && newPixel.a != 1)
                    {
                        continue;
                    }

                    tex.SetPixel(startX + i, startY + j, Color.Lerp(orPixel, newPixel, newPixel.a));
                }
            }
        }

        internal static Vector2 RenderSprite(UITextureAtlas atlas, string spriteName, Color color, Texture2D tex, float textureScale)
        {
            Texture2D readableTexture = atlas.texture.MakeReadable();
            UITextureAtlas.SpriteInfo spriteInfo = atlas[spriteName];
            TextureScaler.scale(readableTexture, (int) (readableTexture.width * textureScale), (int) (readableTexture.height * textureScale));
            int width = readableTexture.width;
            int height = readableTexture.height;
            Color[] colors = readableTexture.GetPixels((int) (spriteInfo.region.xMin * width), (int) (spriteInfo.region.yMin * height), (int) (spriteInfo.region.width * width), (int) (spriteInfo.region.height * height));
            tex.SetPixels((int) (tex.width - (spriteInfo.region.width * width)) / 2, (int) (tex.height - (spriteInfo.region.height * height)) / 2, (int) (spriteInfo.region.width * width), (int) (spriteInfo.region.height * height), colors.Select(x => x * color).ToArray());

            return new Vector2((int) (tex.width - (spriteInfo.region.width * width)) / 2, (int) (tex.height - (spriteInfo.region.height * height)) / 2);
        }

        private static Vector2[] kOutlineOffsets = new Vector2[]
            {
                new Vector2(-1f, -1f),
                new Vector2(-1f, 0),
                new Vector2(-1f, 1f),
                new Vector2(0, 1f),
                new Vector2(1f, -1f),
                new Vector2(1f, 0),
                new Vector2(1f, 1f),
                new Vector2(0, -1f)
            };
        private static float characterSpacing = 0;


    }
}
