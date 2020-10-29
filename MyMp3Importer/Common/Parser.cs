using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyMp3Importer.Common
{
    public class Parser
    {
        public Parser()
        {

        }

        public ParserToken GetParserToken(ObservableCollection<string> list, string tokenName, string token, string path)
        {
            ParserToken parserToken = new ParserToken();
            string container = Helpers.GetContainer(path);

            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            if (list.Contains(token))
                parserToken = new ParserToken() { Name = tokenName, Token = token, State = true };
            else
                parserToken = new ParserToken() { Name = tokenName, Token = token, State = false };

            return parserToken;
        }

        public ParserToken GetParserToken(List<string> list, string tokenName, string token, string path)
        {
            ParserToken parserToken = new ParserToken();
            string container = Helpers.GetContainer(path);

            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            if (list.Contains(token))
                parserToken = new ParserToken() { Name = tokenName, Token = token, State = true };
            else
                parserToken = new ParserToken() { Name = tokenName, Token = token, State = false };

            return parserToken;
        }

        public ParserToken GetParserToken(string tokenName, string path)
        {
            List<string> tokenNames = new List<string>() { "Genre", "Catalog", "Media", "Artist", "Album" };

            string container = Helpers.GetContainer(path);

            var tokens = container.Split('\\').ToList();
            tokens.RemoveAt(0);

            ParserToken parserToken = new ParserToken();

            int counter = 0;
            if (tokenName == "Artist")
            {
                counter = 0;
                foreach (var t in tokens)
                {
                    if (tokenNames[counter] == tokenName)
                        parserToken = new ParserToken() { Name = tokenName, Token = t, State = true };
                    counter++;
                }
                return parserToken;
            }

            if (tokenName == "Album")
            {
                counter = 0;
                foreach (var t in tokens)
                {
                    if (tokenNames[counter] == tokenName)
                        parserToken = new ParserToken() { Name = tokenName, Token = t, State = true };

                    counter++;
                }
                return parserToken;
            }

            return null;
        }
    }
}
