using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyMp3Importer.Common
{
    public static class Helpers
    {
        public static string MD5(string cryptString)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(cryptString);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x2");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public static long DirectoryCount(string startfolder, bool TopDirectoryOnly = true)
        {
            long dirCount = -1;

            if (Directory.Exists(startfolder))
            {
                var option = TopDirectoryOnly == true ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                dirCount = Directory.GetDirectories(startfolder, "*", option).Length;
            }

            return dirCount;
        }

        public static List<string> GetDirectories(string startfolder, bool TopDirectoryOnly = true)
        {
            List<string> list = new List<string>();
            string[] dirs = null;

            if (Directory.Exists(startfolder))
            {
                var option = TopDirectoryOnly == true ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                dirs = Directory.GetDirectories(startfolder, "*", option);
            }

            return dirs.ToList();
        }

        public static string GetContainer(string path)
        {
            string container = "";

            // check if network share or drive letter
            if (path.StartsWith(@"\\"))
            {
                string b = path.Remove(0, 2);
                int start = b.IndexOf("\\") + 1;
                int len = b.Length - start;
                container = b.Substring(start, len);
            }
            else
            {
                string b = path.Remove(0, 3);
                container = b;
            }

            return container;
        }
    }
}

