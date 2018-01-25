using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GestionFormation.Kernel
{
    public static class AssemblyExtentions
    {
        public static IEnumerable<Type> GetAllConcretTypeThatImplementInterface<TInterface>(this Assembly assembly)
        {
            Type[] allConcretTypes;
            try
            {
                allConcretTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                allConcretTypes = ex.Types.Where(a => a != null).ToArray();
            }

            return allConcretTypes.Where(a => a.GetInterface(typeof(TInterface).Name) != null && a.IsClass && !a.IsAbstract);
        }
    }
}