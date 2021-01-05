using System;

namespace MyMp3Importer.Common
{
    public class SongRecord
    {
        public int Genre { get; set; }
        public int Catalog { get; set; }
        public int Media { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Titel { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public DateTime FileDate { get; set; }
        public int FileSize { get; set; }
        public bool IsSample { get; set; }
    }
}
