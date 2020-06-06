using System;
using System.IO;

namespace MyMp3Importer.BLL
{
    public static class Logging
    {
        #region Fields
        private static DateTime _dateTime;
        private static string _logname = "action.log";
        private static string _startPath = AppDomain.CurrentDomain.BaseDirectory;
        private static StreamWriter _writer;
        private static string _messageBuffer = "";
        #endregion

        #region CTOR
        static Logging()
        {
            //_writer = new StreamWriter(Path.Combine(_startPath, _logname), false);
            //_writer.WriteLine(Path.Combine(_startPath, _logname));
        }
        #endregion

        #region Methods

        public static string GetMessages()
        {
            return _messageBuffer;
        }

        public static void Flush()
        {
            //_writer.Flush();
            _messageBuffer = "";
        }

        public static void Log(string txt)
        {
            _dateTime = DateTime.Now;
            _messageBuffer += $"[{_dateTime}]  {txt}\n";
        }
        #endregion
    }
}
