using System;
using System.IO;
using System.Linq;

namespace BatchHollower
{
    class Program
    {
        static readonly string[] _blacklist =
        [
            "Mono.Posix.dll",
            "mscorlib.dll",
            "System.Xml.Linq.dll"
        ];

        static void Main()
        {
            // TODO: launch arguments
            var inputPath = "Managed";
            var outputPath = "Hollowed";

            // Don't load .gitignore files
            var files = new DirectoryInfo(inputPath)
                .GetFiles()
                .Where(x => !_blacklist.Contains(x.Name)
                    && x.Extension == ".dll");

            var hollower = new Hollower();

            foreach (var fi in files)
            {
                var inputFile = fi.FullName;
                var outputFile = GetOutputFilepath(inputFile, outputPath);

                Console.WriteLine($"Hollowing {inputFile}...");                
                hollower.HollowAssembly(inputFile, outputFile);
            }
        }

        static string GetOutputFilepath(string inputFile, string outputPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFile);

            return Path.Combine(
                Environment.CurrentDirectory,
                outputPath,
                $"{fileName}-hollowed.dll");
        }
    }
}