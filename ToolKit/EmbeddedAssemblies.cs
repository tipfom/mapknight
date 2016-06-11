using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace mapKnight.ToolKit {
    public static class EmbeddedAssemblies {
        public static void Serve ( ) {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private static Assembly AssemblyResolve (object sender, ResolveEventArgs args) {
            try {
                Assembly parentAssembly = Assembly.GetExecutingAssembly( );
                string missingAssembly = args.Name.Substring(0, args.Name.IndexOf(','));
                string assemblyFile = missingAssembly + ".dll";
                string resourceName = parentAssembly.GetManifestResourceNames( ).First(a => a.EndsWith(assemblyFile));
                if (!string.IsNullOrWhiteSpace(resourceName)) {
                    using (Stream assemblyFileStream = parentAssembly.GetManifestResourceStream(resourceName)) {
                        byte[ ] assemblyData = new byte[assemblyFileStream.Length];
                        assemblyFileStream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                } else {
                    return null;
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
