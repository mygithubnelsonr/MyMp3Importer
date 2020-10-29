using System.Collections.Generic;

namespace MyMp3Importer.BLL
{
    public sealed class GenreList
    {
        static List<string> genres = null;

        private GenreList()
        {

        }

        private static GenreList instance = null;

        public static GenreList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GenreList();
                    Fill();
                }
                return instance;
            }
        }

        private static void Fill()
        {
            genres = DataGetSet.GetGenres();
        }

        public List<string> Get()
        {
            return genres;
        }
    }
}
