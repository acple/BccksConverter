using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BccksConverter
{
    internal class Program
    {
        private static readonly Encoding _UTF8 = new UTF8Encoding(false);

        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("no parameter");
            if (!args.All(File.Exists))
                throw new Exception("invalid arguments");

            var converter = new Converter();
            var tasks = args.Select(Path.GetFullPath).Distinct().Select(x => Run(x, converter));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static async Task Run(string inputPath, IConverter converter)
        {
            var dir = Path.GetDirectoryName(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var ext = Path.GetExtension(inputPath);
            var outputPath = Path.Combine(dir, $"{fileName}_converted{ext}");

            using var input = new FileStream(inputPath, FileMode.Open);
            using var reader = new StreamReader(input, _UTF8);

            var from = await reader.ReadToEndAsync().ConfigureAwait(false);
            var to = converter.Convert(from);

            using var output = new FileStream(outputPath, FileMode.Create);
            using var writer = new StreamWriter(output, _UTF8);

            await writer.WriteAsync(to).ConfigureAwait(false);
        }
    }
}
