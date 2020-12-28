using System.Collections.Generic;

namespace MyMp3Importer.BLL
{
    public sealed class LogList
    {
        static List<string> logs = null;

        private LogList()
        {

        }

        private static LogList instance = null;

        public static LogList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogList();
                    logs = new List<string>();
                }
                return instance;
            }
        }

        public void Clear()
        {
            logs.Clear();
        }

        public void Write(string entry)
        {
            logs.Add(entry);
        }

        public List<string> Get()
        {
            return logs;
        }
    }
}
