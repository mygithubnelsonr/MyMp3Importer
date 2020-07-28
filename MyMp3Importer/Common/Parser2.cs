using System.Collections.ObjectModel;
using System.Linq;

namespace MyMp3Importer.Common
{
    public class Parser2
    {
        //private List<string> _list;
        //private string _path;
        //private string _tokenName;
        //private string _token;
        //private bool _state;

        //public ObservableCollection<string> list { get; set; }
        //public string path { get; set; }
        //public string tokenName { get; set; }
        //public string token { get; set; }
        //public bool state { get; set; }

        public Parser2()
        {

        }

        //public Parser2(List<string> list, string tokenName, string token, string path)
        //{
        //    //_list = list;
        //    //_path = path;
        //    //_token = token;
        //    //_tokenName = tokenName;
        //    //_state = state;
        //}

        public ParserToken GetParserToken(ObservableCollection<string> list, string tokenName, string token, string path)
        {
            //Parser parser = new Parser();
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


        //public ParserToken GetParserToken()
        //{
        //    Parser parser = new Parser();
        //    ParserToken parserToken = new ParserToken();
        //    string container = Helpers.GetContainer(_path);

        //    var tokens = container.Split('\\').ToList();
        //    tokens.RemoveAt(0);

        //    if (_list.Contains(_token))
        //    {
        //        parserToken = new ParserToken() { Name = _tokenName, Token = _token, State = true };
        //        Debug.Print($"add {_tokenName}, {_token}, {true}");
        //    }
        //    else
        //    {
        //        parserToken = new ParserToken() { Name = _tokenName, Token = _token, State = false };
        //        Debug.Print($"add {_tokenName}, {_token}, {false}");
        //    }

        //    return parserToken;
        //}
    }
}
