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
    public interface IConverter
    {
        string Convert(string input);
    }

    public class Converter : IConverter
    {
        private static readonly Parser<char, string> _Parser;

        static Converter()
        {
            var openBrace = Char('{');
            var closeBrace = Char('}');
            var pipe = Char('|');
            var withRuby =
                from _ in openBrace
                from words in ManyTill(Any(), pipe).ToStr()
                from ruby in ManyTill(Any(), closeBrace).ToStr()
                select $"{{{words}}}({ruby})";

            var asterisk = Char('*');
            var surrogate = HighSurrogate().Append(LowSurrogate()).ToStr();
            var strong = asterisk.Right(ManyTill(surrogate | Any().ToStr(), asterisk))
                .Map(chars => chars
                    .Select(x => $"{{{x}}}({'﹅'})")
                    .Aggregate(new StringBuilder(), (sb, x) => sb.Append(x))
                    .ToString());

            var hat = Char('^');
            var tcy = hat.Right(ManyTill(Any(), hat)).ToStr()
                .Map(words => $"[tcy]{words}[/tcy]");

            var parser = Many(withRuby | strong | tcy | Any().ToStr()).Join();

            _Parser = parser;
        }

        public string Convert(string input)
            => _Parser.Parse(input);

        public string Convert(Stream input)
            => _Parser.Parse(input);

        public string Convert(Stream input, Encoding encoding)
            => _Parser.Parse(input, encoding);
    }
}
