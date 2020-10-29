using MyMp3Importer.BLL;
using System.Collections.Generic;
using System.Linq;

namespace MyMp3Importer.Common
{
    public class ParserTokenList : List<ParserToken>
    {
        List<string> genres = null;
        List<string> catalogs = null;
        List<string> medias = null;

        string _startpath = "";

        public ParserTokenList()
        {

        }
        public ParserTokenList(string startpath)
        {
            _startpath = startpath;
        }

        public List<ParserToken> Get()
        {
            ParserTokenList tokenList = new ParserTokenList();
            ParserToken parserToken = new ParserToken();
            Parser parser = new Parser();

            genres = GenreList.Instance.Get();
            catalogs = CatalogList.Instance.Get();
            medias = MediaList.Instance.Get();

            string container = Helpers.GetContainer(_startpath);
            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            // processing genre
            parserToken = parser.GetParserToken(genres, "Genre", tokens[0], _startpath);
            tokenList.Add(parserToken);

            // processing catalog
            parserToken = parser.GetParserToken(catalogs, "Catalog", tokens[1], _startpath);
            tokenList.Add(parserToken);

            // processing media
            parserToken = parser.GetParserToken(medias, "Media", tokens[2], _startpath);
            tokenList.Add(parserToken);

            // processing artist
            parserToken = parser.GetParserToken("Artist", _startpath);
            tokenList.Add(parserToken);

            // processing album
            parserToken = parser.GetParserToken("Album", _startpath);
            tokenList.Add(parserToken);

            return tokenList;
        }
    }
}
