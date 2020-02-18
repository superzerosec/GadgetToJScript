using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.IO;

namespace GadgetToJScript
{
    class TestAssemblyLoader
    {
        public static Assembly compile(string InputFile, string ReferenceAssemblies)
        {
            // Shellcode loader would make more sense here, just make sure your code is located within the default constructor.
            string _testClass = File.ReadAllText(InputFile);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("System.dll");

            if (!string.IsNullOrEmpty(ReferenceAssemblies))
            {
                var assemblies = ReferenceAssemblies.Split(',');

                foreach (var asm in assemblies)
                    parameters.ReferencedAssemblies.Add(asm);
                    
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, _testClass);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}: {2}", error.ErrorNumber, error.ErrorText, error.Line));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            Assembly _compiled = results.CompiledAssembly;

            return _compiled;
        }

    }
}
