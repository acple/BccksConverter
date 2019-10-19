using System.IO;
using System.Linq;
using System.Text;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

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
            var any = Any().AsString();

            var openBrace = Char('{');
            var closeBrace = Char('}');
            var pipe = Char('|');
            var withRuby =
                from words in Quoted(openBrace, LookAhead(pipe)).AsString()
                from ruby in Quoted(pipe, closeBrace).AsString()
                select $"{{{words}}}({ruby})";

            var asterisk = Char('*');
            var strong = (SurrogatePair() | any).Quote(asterisk)
                .Map(chars => chars.Select(x => $"{{{x}}}({'﹅'})"))
                .Join();

            var trailingWhiteSpace = SkipTill(WhiteSpace(), EndOfLine()).AsString();

            var hat = Char('^');
            var tcy = Quoted(hat).AsString()
                .Map(words => $"[tcy]{words}[/tcy]");

            var parser = Choice(withRuby, strong, tcy, trailingWhiteSpace, any)
                .FoldLeft(_ => new StringBuilder(), (builder, x) => builder.Append(x))
                .ToStr();

            _Parser = parser;
        }

        public string Convert(string input)
            => _Parser.Parse(input).Value;

        public string Convert(Stream input)
            => _Parser.Parse(input).Value;

        public string Convert(Stream input, Encoding encoding)
            => _Parser.Parse(input, encoding).Value;
    }
}
