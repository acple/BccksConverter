using System.Globalization;
using System.IO;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace BccksConverter
{
    public class Converter
    {
        private static readonly Parser<char, string> _parser;

        static Converter()
        {
            var openBrace = Char('{');
            var closeBrace = Char('}');
            var pipe = Char('|');
            var withRuby = from _ in openBrace
                           from words in ManyTill(Any(), pipe).ToStr()
                           from ruby in ManyTill(Any(), closeBrace).ToStr()
                           select $"{{{words}}}({ruby})";

            var aster = Char('*');
            var strong = from start in aster
                         from words in ManyTill(Any(), aster).ToStr()
                         select ConvertStrong(words);

            var hat = Char('^');
            var tcy = from _ in hat
                      from words in ManyTill(Any(), hat).ToStr()
                      select $"[tcy]{words}[/tcy]";

            var parser = Many(withRuby | strong | tcy | Any().ToStr()).Join();

            _parser = parser;
        }

        private static string ConvertStrong(string words)
            => $"{{{words}}}({new string('﹅', CountWord(words))})";

        private static int CountWord(string words)
            => new StringInfo(words).LengthInTextElements;

        public string Convert(string input)
            => _parser.Parse(input);

        public string Convert(Stream input)
            => _parser.Parse(input);
    }
}
