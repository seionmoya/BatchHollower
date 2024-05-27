using System.IO;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace BatchHollower
{
    public class Hollower
    {
        public void HollowAssembly(string inputFile, string outputFile)
        {
            using (var assembly = GetAssemblyDefinition(inputFile))
            {
                var types = assembly.MainModule.GetAllTypes();

                foreach (var type in types)
                {
                    HollowType(type);
                }

                assembly.Write(outputFile);
            }
        }

        public AssemblyDefinition GetAssemblyDefinition(string inputFile)
        {
            var inputPath = Path.GetDirectoryName(inputFile);
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(inputPath);

            var parameters = new ReaderParameters
            {
                AssemblyResolver = resolver
            };

            return AssemblyDefinition.ReadAssembly(inputFile, parameters);
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