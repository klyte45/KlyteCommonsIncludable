using System;
using System.IO;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public static class TextureUtils
    {
        public static Texture2D DeCompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Default);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        public static void DumpTo(this Texture2D texture, string filename)
        {
            byte[] bytes;
            try
            {
                bytes = GetBytes(texture);
            }
            catch
            {
                try
                {
                    var img = texture.MakeReadable();
                    bytes = GetBytes(img);
                }
                catch (Exception ex)
                {
                    LogUtils.DoErrorLog("There was an error while dumping the texture - " + ex.Message);
                    return;
                }
            }

            File.WriteAllBytes(filename, bytes);
            LogUtils.DoWarnLog($"Texture dumped to \"{filename}\"");
        }

        private static byte[] GetBytes(Texture2D img)
        {
            byte[] bytes = img.EncodeToPNG();
            if (bytes is null)
            {
                bytes = img.DeCompress().EncodeToPNG();
            }

            return bytes;
        }
    }
}
