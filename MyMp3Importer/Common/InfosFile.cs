using System;

namespace MyMp3Importer.Common
{
    public class FileDetails
    {
        public string File { get; set; }
        public long Size { get; set; }
        public DateTime LastWrite { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
    }
}
