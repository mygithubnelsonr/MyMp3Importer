using MyMp3Importer.BLL;
using System.Collections.Generic;
using System.Linq;

namespace MyMp3Importer.Common
{
    public class Parser
    {
        private List<string> pathTokens = new List<string>() { "Genre", "Catalog", "Media", "Interpret", "Album" };

        public Parser()
        { }

        public List<ParserToken> ParserTokens(string path)
        {

            List<string> genres = null;
            List<string> catalogs = null;
            List<string> medias = null;
            List<string> interpreters = null;
            List<string> albums = null;

            genres = DataGetSet.GetGenres();
            catalogs = DataGetSet.GetCatalogs();
            medias = DataGetSet.GetMedia();
            //interpreters = DataGetSet.GetInterpreters();
            //albums = DataGetSet.GetAlbums();

            List<ParserToken> list = new List<ParserToken>();

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

            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            int counter = 0;
            foreach (var t in tokens)
            {
                if (genres.Contains(t) && pathTokens[counter] == "Genre")
                    list.Add(new ParserToken() { Name = "Genre", Token = t, State = true });
                else if (!genres.Contains(t) && pathTokens[counter] == "Genre")
                    list.Add(new ParserToken() { Name = "Genre", Token = t, State = false });

                if (catalogs.Contains(t) && pathTokens[counter] == "Catalog")
                    list.Add(new ParserToken() { Name = "Catalog", Token = t, State = true });
                else if (!catalogs.Contains(t) && pathTokens[counter] == "Catalog")
                    list.Add(new ParserToken() { Name = "Catalog", Token = t, State = false });

                if (medias.Contains(t) && pathTokens[counter] == "Media")
                    list.Add(new ParserToken() { Name = "Media", Token = t, State = true });
                else if (!medias.Contains(t) && pathTokens[counter] == "Media")
                    list.Add(new ParserToken() { Name = "Media", Token = t, State = false });

                if (pathTokens[counter] == "Interpret")
                    list.Add(new ParserToken() { Name = "Interpret", Token = t, State = true });

                if (pathTokens[counter] == "Album")
                    list.Add(new ParserToken() { Name = "Album", Token = t, State = true });

                counter++;
            }
            return list;
        }
    }
}
