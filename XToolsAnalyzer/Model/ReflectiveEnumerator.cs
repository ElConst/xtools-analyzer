using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XToolsAnalyzer.Model
{
    /// <summary>Helps to get instances of classes derived from some base class.</summary>
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        /// <summary>Looks for classes derived from "T" and creates an instance for each of them.</summary>
        /// <typeparam name="T">Base class.</typeparam>
        /// <param name="constructorArgs">Arguments for constructors of derived classes.</param>
        /// <returns>Collection of instances of classes derived from the base class.</returns>
        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> instances = new List<T>();

            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                instances.Add((T)Activator.CreateInstance(type, constructorArgs));
            }

            return instances;
        }
    }
}
