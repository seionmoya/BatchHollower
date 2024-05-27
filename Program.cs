using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

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
            var inputPath = "Managed";
            var outputPath = "Hollowed";

            // Don't load .gitignore files
            var files = new DirectoryInfo(inputPath)
                .GetFiles()
                .Where(x => !_blacklist.Contains(x.Name)
                    && x.Extension == ".dll");

            foreach (var fi in files)
            {
                var filePath = fi.FullName;
                Console.WriteLine($"Hollowing {filePath}...");
                HollowAssembly(inputPath, outputPath, filePath);
            }
        }

        static void HollowAssembly(string inputPath, string outputPath, string filePath)
        {
            using var assembly = GetAssemblyDefinition(inputPath, filePath);
            var types = assembly.MainModule.GetAllTypes();

            foreach (var type in types)
            {
                HollowMethods(type);
                HollowProperties(type);
            }

            assembly.Write(GetOutputFilepath(outputPath, filePath));
        }

        static AssemblyDefinition GetAssemblyDefinition(string inputPath, string filePath)
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(inputPath);

            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = assemblyResolver
            };

            return AssemblyDefinition.ReadAssembly(filePath, readerParameters);
        }

        static void HollowMethods(TypeDefinition type)
        {
            if (!type.HasMethods)
            {
                return;
            }

            foreach (var method in type.Methods)
            {
                HollowMethod(method);
            }
        }

        static void HollowProperties(TypeDefinition type)
        {
            if (!type.HasProperties)
            {
                return;
            }

            foreach (var property in type.Properties)
            {
                if (property.GetMethod != null)
                {
                    HollowMethod(property.GetMethod);
                }

                if (property.SetMethod != null)
                {
                    HollowMethod(property.SetMethod);
                }
            }
        }

        static void HollowMethod(MethodDefinition method)
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

        static string GetOutputFilepath(string outputPath, string filePath)
        {
            return Path.Combine(
                Environment.CurrentDirectory,
                outputPath,
                Path.GetFileNameWithoutExtension(filePath) + "-hollowed.dll");
        }
    }
}