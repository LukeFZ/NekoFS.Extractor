using LibNekoFS.Native;
using LibNekoFS.NekoData;

namespace NekoFS.Extractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: NekoFS.Extractor.exe <path to folder containing .nekodata files>");
                return;
            }

            var path = args[0];

            var output = path + @"\output_resources";
            Directory.CreateDirectory(output);

            var fs = new NativeFileSystem(path);

            foreach (var file in Directory.GetFiles(path, "*.nekodata", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(file);
                Console.WriteLine($"Parsing file {fileName}.");

                var nekoFile = fs.Open(fileName);
                var nekoFs = new NekoDataFileSystem(nekoFile);
                nekoFs.Init();

                var outDir = Path.Join(output, Path.GetFileNameWithoutExtension(file));
                Directory.CreateDirectory(outDir);

                foreach (var nekoFsFileName in nekoFs.GetFiles())
                {
                    var nekoFsFile = nekoFs.Open(nekoFsFileName);
                    if (nekoFsFileName.Contains('/'))
                        Directory.CreateDirectory(Path.Join(outDir, nekoFsFileName[..nekoFsFileName.LastIndexOf("/", StringComparison.Ordinal)]));

                    using var nekoStream = nekoFsFile.NewReadStream();
                    using var fileOutStream = File.OpenWrite(Path.Join(outDir, nekoFsFileName));
                    nekoStream.CopyTo(fileOutStream);
                    Console.WriteLine($"Successfully extracted file {nekoFsFileName}!");
                }
            }
        }
    }
}