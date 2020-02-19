using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

using Microsoft.CSharp;

namespace GadgetToJScript
{
    class AssemblyLoader
    {
        public static Assembly Compile(string InputFile, string ReferenceAssemblies)
        {
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