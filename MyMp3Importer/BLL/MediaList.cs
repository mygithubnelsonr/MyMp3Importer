using System.Collections.Generic;

namespace MyMp3Importer.BLL
{
    public sealed class MediaList
    {
        static List<string> medias = null;

        private MediaList()
        {

        }

        private static MediaList instance = null;

        public static MediaList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MediaList();
                    Fill();
                }
                return instance;
            }
        }

        private static void Fill()
        {
            medias = DataGetSet.GetCatalogs();
        }

        public List<string> Get()
        {
            return medias;
        }
    }
}
