using MyMp3Importer.BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyMp3Importer.Common
{
    public static class Helpers
    {
        public static void TextBoxSearchClear(TextBox textBox)
        {
            textBox.Text = SettingsDb.PlaceHolderText;
            textBox.Foreground = Brushes.Gray;
        }

        public static bool IsQuery(string filter)
        {
            bool IsQuery = false;

            if (filter == "" || filter == SettingsDb.PlaceHolderText)
                IsQuery = false;
            else
                IsQuery = true;

            return IsQuery;
        }

        public static string GetQueryString(string queryText)
        {
            List<string> tokens = new List<string>();
            string[] arTokens = null;
            string sql = "";
            bool findExplizit = false;

            try
            {
                arTokens = queryText.Split('=');

                if (queryText.IndexOf(@"==") == -1)
                    findExplizit = false;
                else
                {
                    findExplizit = true;
                    arTokens = arTokens.Where(w => w != arTokens[1]).ToArray();
                }

                var search = arTokens[0];
                var argument = arTokens[1];

                if (argument.Substring(0, 1) == "'")
                    argument = argument.Substring(1);

                if (argument.Substring(argument.Length - 1, 1) == "'")
                    argument = argument.Substring(0, argument.Length - 1);

                if (findExplizit == true)
                    sql = $"select * from vSongs where {search} = '{argument}'";
                else
                    if (search == "*")
                    sql = $"select * from vsongs where charindex('{argument}', lower( concat([pfad],[filename]))) > 0";
                else
                    sql = $"select * from vSongs where {search} like '%{argument}%'";

                return sql;
            }
            catch
            {
                return null;
            }
        }

        public static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
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
    }
}

