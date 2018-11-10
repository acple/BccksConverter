using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BccksConverter
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("no parameter");
            if (!args.All(File.Exists))
                throw new Exception("invalid arguments");

            var tasks = args.Select(Path.GetFullPath).Distinct().Select(x => Run(x));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static async Task Run(string inputPath)
        {
            var dir = Path.GetDirectoryName(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var ext = Path.GetExtension(inputPath);
            var outputPath = Path.Combine(dir, $"{fileName}_converted{ext}");

            using (var input = new FileStream(inputPath, FileMode.Open))
            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                var from = await reader.ReadToEndAsync().ConfigureAwait(false);
                var to = new Converter().Convert(from);

                using (var output = new FileStream(outputPath, FileMode.Create))
                using (var writer = new StreamWriter(output, Encoding.UTF8))
                    await writer.WriteAsync(to).ConfigureAwait(false);
            }
        }
    }
}
