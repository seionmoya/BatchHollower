using System;
using System.IO;
using System.Linq;

namespace Seion.BatchHollower
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

            var files = new DirectoryInfo(inputPath).GetFiles();
            var hollower = new Hollower();

            foreach (var fi in files)
            {
                var inputFile = fi.FullName;
                var outputFile = GetOutputFilepath(inputFile, outputPath);

                if (fi.Extension != ".dll" || _blacklist.Contains(fi.Name)) 
                {
                    Console.WriteLine($"Skipping {inputFile}...");    
                    continue;
                }

                Console.WriteLine($"Hollowing {inputFile}...");                
                hollower.HollowAssembly(inputFile, outputFile);
            }
        }

        static string GetOutputFilepath(string inputFile, string outputPath)
        {
            var fileName = Path.GetFileName(inputFile);

            return Path.Combine(
                Environment.CurrentDirectory,
                outputPath,
                fileName);
        }
    }
}