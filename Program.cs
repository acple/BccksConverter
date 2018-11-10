using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BccksConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("no parameter");

            var sourcePath = args[0];
            var fullPath = Path.GetFullPath(sourcePath);
            var dir = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var ext = Path.GetExtension(fullPath);
            var outputPath = Path.Combine(dir, $"{fileName}_converted{ext}");

            using (var input = new FileStream(sourcePath, FileMode.Open))
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
