using MyMp3Importer.BLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMp3Importer.Common
{
    public class Parser
    {
        private List<string> _genres;
        private List<string> _catalogs;
        private List<string> _medias;

        private bool _isSampler = false;

        private List<string> pathTokens = new List<string>() { "Genre", "Catalog", "Media", "Interpret", "Album" };
        private Hashtable hashtable;

        // ToDo try usebility of hashtable (s.o.) in Parser or scanner

        public Parser()
        {

        }

        public Parser(bool isSampler)
        {
            _isSampler = isSampler;
            hashtable = new Hashtable() { { "Genre", 1 }, { "Catalog", 2 }, { "Media", 3 }, { "Interpret", 4 }, { "Album", 5 } };

            _genres = DataGetSet.GetGenres();
            _catalogs = DataGetSet.GetCatalogs();
            _medias = DataGetSet.GetMedia();
        }

        public async Task<List<ParserToken>> ParesSamplerTokensAsync(string path)
        {
            List<string> genres = await DataGetSet.GetGenresAsync();
            List<string> catalogs = await DataGetSet.GetCatalogsAsync();
            List<string> medias = await DataGetSet.GetMediaAsync();

            List<ParserToken> list = new List<ParserToken>();

            string container = Helpers.GetContainer(path);

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

        //private Parser2 parserToken(List<string> list, string tokenName, string token, string path)
        //{
        //    Parser2 parser = new Parser2(list, tokenName, token, path);

        //    string container = GetContainer(path);

        //    var tokens = container.Split('\\').ToList();
        //    tokens.RemoveAt(0);

        //    if (list.Contains(token))
        //    {
        //        var p = parser.GetParserToken();
        //        Debug.Print($"add {tokenName}, {token}, {true}");
        //    }
        //    else
        //    {
        //        var p = parser.GetParserToken();      // = new ParserToken2() { Name = tokenName, Token = token, State = false };
        //        Debug.Print($"add {tokenName}, {token}, {false}");
        //    }

        //    return parser;
        //}

        public async Task<List<ParserToken>> ParserTokensAsync(string path)
        {
            List<string> genres = await DataGetSet.GetGenresAsync();
            List<string> catalogs = await DataGetSet.GetCatalogsAsync();
            List<string> medias = await DataGetSet.GetMediaAsync();

            string container = Helpers.GetContainer(path);

            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            List<ParserToken> list = new List<ParserToken>();
            int counter = 0;

            try
            {
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
            catch
            {
                return null;
            }
        }
    }
}
