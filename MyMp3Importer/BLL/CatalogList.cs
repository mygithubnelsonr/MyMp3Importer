using System.Collections.Generic;

namespace MyMp3Importer.BLL
{
    public sealed class CatalogList
    {
        static List<string> catalogs = null;

        private CatalogList()
        {

        }

        private static CatalogList instance = null;

        public static CatalogList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CatalogList();
                    Fill();
                }
                return instance;
            }
        }

        private static void Fill()
        {
            catalogs = DataGetSet.GetCatalogs();
        }

        public List<string> Get()
        {
            return catalogs;
        }
    }
}
