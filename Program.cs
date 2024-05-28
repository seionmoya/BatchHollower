using System;
using System.IO;

namespace Seion.BatchHollower
{
    class Program
    {
        static readonly string[] _blacklist =
        [
            "Mono.",            // All mono dlls
            "mscorlib.dll",     // Single match
            "netstandard.dll",  // Single match
            "System.",          // All system dlls
            "UnityEngine."      // All unityengine dlls
        ];

        static void Main()
        {
            var hollower = new Hollower();

            // TODO: launch arguments
            var inputPath = "Managed";
            var outputPath = "Hollowed";

            // TODO: support child folder lookup
            var files = new DirectoryInfo(inputPath).GetFiles();

            foreach (var fi in files)
            {
                var inputFile = fi.FullName;
                var outputFile = GetOutputFilepath(inputFile, outputPath);

                if (IsOnBlacklist(fi)) 
                {
                    Console.WriteLine($"Skipping {inputFile}...");    
                    continue;
                }

                Console.WriteLine($"Hollowing {inputFile}...");                
                hollower.HollowAssembly(inputPath, inputFile, outputFile);
            }
        }

        static bool IsOnBlacklist(FileInfo fi)
        {
            if (fi.Extension != ".dll")
            {
                return true;
            }

            foreach (var name in _blacklist)
            {
                if (fi.Name.StartsWith(name))
                {
                    return true;
                }
            }

            return false;
        }

        static string GetOutputFilepath(string inputFile, string outputPath)
        {
            var fileName = Path.GetFileName(inputFile);

            // TODO: support child folder lookup
            return Path.Combine(
                Environment.CurrentDirectory,
                outputPath,
                fileName);
        }
    }
}