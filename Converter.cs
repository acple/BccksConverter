using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

            var asterisk = Char('*');
            var surrogate = HighSurrogate().Append(LowSurrogate()).ToStr();
            var strong = from _ in asterisk
                         from chars in ManyTill(surrogate | Any().ToStr(), asterisk)
                         select chars
                            .Select(x => $"{{{x}}}({'﹅'})")
                            .Aggregate(new StringBuilder(), (sb, x) => sb.Append(x))
                            .ToString();

            var hat = Char('^');
            var tcy = from _ in hat
                      from words in ManyTill(Any(), hat).ToStr()
                      select $"[tcy]{words}[/tcy]";

            var parser = Many(withRuby | strong | tcy | Any().ToStr()).Join();

            _parser = parser;
        }

        public string Convert(string input)
            => _parser.Parse(input);

        public string Convert(Stream input)
            => _parser.Parse(input);
    }
}
