using System.IO;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace BatchHollower
{
    public class Hollower
    {
        public void HollowAssembly(string inputFile, string outputFile)
        {
            var inputPath = Path.GetDirectoryName(inputFile);
            using var assembly = GetAssemblyDefinition(inputPath, inputFile);
            var types = assembly.MainModule.GetAllTypes();

            foreach (var type in types)
            {
                HollowType(type);
            }

            assembly.Write(outputFile);
        }

        public AssemblyDefinition GetAssemblyDefinition(string inputPath, string filePath)
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(inputPath);

            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = assemblyResolver
            };

            return AssemblyDefinition.ReadAssembly(filePath, readerParameters);
        }

        public void HollowType(TypeDefinition type)
        {
            HollowMethods(type);
            HollowProperties(type);
        }

        public void HollowMethods(TypeDefinition type)
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

        public void HollowProperties(TypeDefinition type)
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

        public void HollowMethod(MethodDefinition method)
        {
            if (method.HasBody)
            {
                method.Body.Instructions.Clear();
            }
        }
    }
}