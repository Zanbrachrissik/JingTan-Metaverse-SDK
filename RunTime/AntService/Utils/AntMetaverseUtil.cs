using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ant.MetaVerse
{
    public class AntMetaverseUtil{
        public static Assembly GetLoadedAssembly(string assemblyName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return assembly;
                }
            }

            return null;
        }
    }
}