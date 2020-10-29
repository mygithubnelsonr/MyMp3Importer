using NRSoft.FunctionPool;
using System.Collections.Generic;
using System.IO;

namespace MyMp3Importer.Common
{
    public class FileDetailsList : List<FileDetails>
    {
        private long _dirCount = -1;
        private int _fileCount = -1;
        private long _allFileSize = -1;
        private string _startDirectory = "";
        private string _filePattern = "";
        private FileDetailsList _fileDetailsList = null;

        private List<string> _fileList = null;

        public FileDetailsList()
        {

        }

        public FileDetailsList(string startDirectory, string filePattern)
        {
            _startDirectory = startDirectory;
            _filePattern = filePattern;
        }

        public FileDetailsList Load()
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            FileDetailsList _fileDetailsList = new FileDetailsList();

            fileInfos = FileSystemUtils.GetFileinfos(_startDirectory, true);

            foreach (FileInfo fi in fileInfos)
            {
                if (fi.Extension == _filePattern)
                {
                    string file = GeneralH.ToProperCase(fi.Name.Replace(fi.Extension, ""));
                    _fileDetailsList.Add(new FileDetails() { File = file, Extension = fi.Extension, Path = fi.DirectoryName, Size = fi.Length, LastWrite = fi.LastWriteTime }); ;
                    _allFileSize += fi.Length;
                }
            }
            _dirCount = Helpers.DirectoryCount(_startDirectory, false);
            _fileCount = _fileDetailsList.Count;

            return _fileDetailsList;
        }

        public FileDetailsList(List<string> fileList, string startDirectory, string filePattern)
        {
            _startDirectory = startDirectory;
            _filePattern = filePattern;
            _fileList = fileList;
        }

        public FileDetailsList LoadFiles()
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            FileDetailsList _fileDetailsList = new FileDetailsList();

            foreach (string file in _fileList)
            {
                FileInfo fi = FileSystemUtils.GetFileinfo(file);
                string f = GeneralH.ToProperCase(fi.Name.Replace(fi.Extension, ""));
                _fileDetailsList.Add(new FileDetails()
                { 
                    File = f, 
                    Extension = fi.Extension,
                    Path = fi.DirectoryName,
                    Size = fi.Length,
                    LastWrite = fi.LastWriteTime
                });
                _allFileSize += fi.Length;
            }

            _dirCount = Helpers.DirectoryCount(_startDirectory, false);
            _fileCount = _fileDetailsList.Count;

            return _fileDetailsList;
        }

        public long DirCount
        {
            get { return _dirCount; }
        }

        public int FileCount
        {
            get { return _fileCount; }
        }

        public long FileSizeAll
        {
            get { return _allFileSize; }
        }

        public FileDetailsList FileDetails
        {
            get { return _fileDetailsList; }
        }
    }
}
