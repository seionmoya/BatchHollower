using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace BatchHollower
{
    class Program
    {
        static readonly string[] _blacklist =
        [
            "Mono.Posix.dll",
            "mscorlib.dll",
            "System.Xml.Linq.dll",
        ];

        static void Main()
        {
            var files = new DirectoryInfo("./Managed")
                .GetFiles()
                .Where(x => !_blacklist.Contains(x.Name)
                    && x.Extension == ".dll");

            foreach (var fi in files)
            {
                var filepath = fi.FullName;

                Console.WriteLine($"Hollowing {filepath}...");

                try
                {
                    HollowAssembly(filepath);
                }
                catch (AssemblyResolutionException)
                {
                    Console.Error.WriteLine("WARNING: Cannot be resolved, might be obfuscated.");
                }
            }
        }

        static void HollowAssembly(string filepath)
        {
            var data = File.ReadAllBytes(filepath);
            using var ms = new MemoryStream(data);
            var module = ModuleDefinition.ReadModule(ms);

            foreach (var type in module.Types)
            {
                if (!type.HasMethods)
                {
                    continue;
                }

                foreach (var method in type.Methods)
                {
                    try
                    {
                        if (method.HasBody)
                        {
                            method.Body.Instructions.Clear();
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Something gone wrong inside Mono.Cecil.
                        // IL appears to be cleared.
                    }
                }
            }

            module.Write();
            File.WriteAllBytes(filepath, ms.ToArray());
        }
    }
}