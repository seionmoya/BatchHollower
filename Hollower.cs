using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Seion.BatchHollower
{
    public class Hollower
    {
        public AssemblyDefinition GetAssemblyDefinition(string inputPath, string inputFile)
        {
            var resolver = new DefaultAssemblyResolver();
            var parameters = new ReaderParameters();

            resolver.AddSearchDirectory(inputPath);            
            parameters.AssemblyResolver = resolver;

            return AssemblyDefinition.ReadAssembly(inputFile, parameters);
        }

        public void HollowAssembly(string inputPath, string inputFile, string outputFile)
        {
            using (var assembly = GetAssemblyDefinition(inputPath, inputFile))
            {
                var types = assembly.MainModule.GetAllTypes();

                foreach (var type in types)
                {
                    HollowType(type);
                }

                assembly.Write(outputFile);
            }
        }

        public void HollowType(TypeDefinition type)
        {
            HollowMethods(type);
            HollowProperties(type);
        }

        public void HollowMethods(TypeDefinition type)
        {
            if (type == null)
            {
                return;
            }

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
            if (type == null)
            {
                return;
            }

            if (!type.HasProperties)
            {
                return;
            }

            foreach (var property in type.Properties)
            {
                HollowMethod(property.GetMethod);
                HollowMethod(property.SetMethod);
            }
        }

        public void HollowMethod(MethodDefinition method)
        {
            if (method == null)
            {
                return;
            }

            if (!method.HasBody)
            {
                return;
            }

            method.Body.Instructions.Clear();
            method.Body.Variables.Clear();
        }
    }
}