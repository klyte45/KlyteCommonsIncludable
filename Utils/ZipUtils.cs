using System.IO;
using System.IO.Compression;
using System.Text;

namespace Klyte.Commons.Utils
{
    public class ZipUtils
    {
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str) => ZipBytes(Encoding.UTF8.GetBytes(str));

        public static byte[] ZipBytes(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                CopyTo(msi, gs);
                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes) => Encoding.UTF8.GetString(UnzipBytes(bytes));

        public static byte[] UnzipBytes(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
                return mso.ToArray();
            }
        }
    }
}
