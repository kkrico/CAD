using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Cad.Web
{
    public class UnityConfig
    {
        private const string ApplicationNamespace = "CAD";
        private static readonly Dictionary<Type, HashSet<Type>> InternalTypeMapping = new Dictionary<Type, HashSet<Type>>();

        #region Unity Container
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterConventions(container);
            RegisterTypes(container);
            return container;
        });
        #endregion

        private static void RegisterConventions(UnityContainer container, IEnumerable<Assembly> assemblies = null)
        {
            foreach (var type in GetClassesFromAssemblies(assemblies))
            {
                var interfacesToBeRegsitered = GetInterfacesToBeRegistered(type);
                AddToInternalTypeMapping(type, interfacesToBeRegsitered);
            }


            foreach (var typeMapping in InternalTypeMapping)
            {
                if (typeMapping.Value.Count == 1)
                {
                    var type = typeMapping.Value.First();
                    container.RegisterType(typeMapping.Key, type);
                }
                else
                {
                    foreach (var type in typeMapping.Value)
                    {
                        container.RegisterType(typeMapping.Key, type, GetNameForRegsitration(type));
                    }
                }
            }
        }

        private static void AddToInternalTypeMapping(Type type, IEnumerable<Type> interfacesOnType)
        {
            foreach (var interfaceOnType in interfacesOnType)
            {
                if (!InternalTypeMapping.ContainsKey(interfaceOnType))
                {
                    InternalTypeMapping[interfaceOnType] = new HashSet<Type>();
                }

                InternalTypeMapping[interfaceOnType].Add(type);
            }
        }

        private static IEnumerable<Type> GetInterfacesToBeRegistered(Type type)
        {
            var allInterfacesOnType = type.GetInterfaces()
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i).ToList();

            return allInterfacesOnType.Except(allInterfacesOnType.SelectMany(i => i.GetInterfaces())).ToList();
        }

        private static string GetNameForRegsitration(Type type)
        {
            var name = type.Name;

            return name;
        }

        private static IEnumerable<Type> GetClassesFromAssemblies(IEnumerable<Assembly> assemblies = null)
        {
            var allClasses = assemblies != null ? AllClasses.FromAssemblies(assemblies) : AllClasses.FromAssembliesInBasePath();
            var d =
                allClasses.Where(
                    n =>
                        n.Namespace != null &&
                        !n.Namespace.StartsWith("System", StringComparison.InvariantCultureIgnoreCase));
            return
                allClasses.Where(
                    n =>
                        n.Namespace != null
                        && n.Namespace.StartsWith(ApplicationNamespace, StringComparison.InvariantCultureIgnoreCase));
        }

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
        }
    }

    public class AllClasses
    {
        private static readonly string NetFrameworkProductName = GetNetFrameworkProductName();
        private static readonly string UnityProductName = GetUnityProductName();

        private static string GetUnityProductName()
        {
            var productAttribute = typeof(UnityContainer)
                .GetCustomAttributes(typeof(AssemblyProductAttribute), true);

            if (!productAttribute.Any()) return null;

            var unity = (AssemblyProductAttribute)productAttribute.First();
            return unity.Product;
        }

        private static string GetNetFrameworkProductName()
        {

            var productAttribute = typeof(object)
                .GetCustomAttributes(typeof(AssemblyProductAttribute), true);

            if (!productAttribute.Any()) return null;

            var assembly = (AssemblyProductAttribute)productAttribute.First();
            return assembly.Product;
        }

        public static IEnumerable<Type> FromLoadedAssemblies(bool includeSystemAssemblies = false, bool includeUnityAssemblies = false, bool includeDynamicAssemblies = false, bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetLoadedAssemblies(includeSystemAssemblies, includeUnityAssemblies, includeDynamicAssemblies), skipOnError);
        }

        public static IEnumerable<Type> FromAssembliesInBasePath(bool includeSystemAssemblies = false, bool includeUnityAssemblies = false, bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetAssembliesInBasePath(includeSystemAssemblies, includeUnityAssemblies, skipOnError), skipOnError);
        }

        private static IEnumerable<Assembly> GetAssembliesInBasePath(bool includeSystemAssemblies, bool includeUnityAssemblies, bool skipOnError)
        {
            string basePath;

            try
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
                basePath = basePath + "bin";
            }
            catch (SecurityException)
            {
                if (!skipOnError)
                {
                    throw;
                }

                return new Assembly[0];
            }

            return GetAssemblyNames(basePath, skipOnError)
                    .Select(an => LoadAssembly(Path.GetFileNameWithoutExtension(an), skipOnError))
                    .Where(a => a != null && (includeSystemAssemblies || !IsSystemAssembly(a)) &&
                        (includeUnityAssemblies || !IsUnityAssembly(a)));
        }

        private static bool IsSystemAssembly(Assembly a)
        {

            if (NetFrameworkProductName != null)
            {
                var productAttribute = a.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                if (!productAttribute.Any()) return false;

                var product = (AssemblyProductAttribute)productAttribute.First();
                return string.Compare(NetFrameworkProductName, product.Product, StringComparison.Ordinal) == 0;
            }
            else
            {
                return false;
            }
        }

        private static bool IsUnityAssembly(Assembly a)
        {
            if (UnityProductName != null)
            {
                var productAttribute = a.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                if (!productAttribute.Any()) return false;

                var product = (AssemblyProductAttribute)productAttribute.First();
                return string.Compare(UnityProductName, product.Product, StringComparison.Ordinal) == 0;
            }
            else
            {
                return false;
            }
        }

        private static IEnumerable<string> GetAssemblyNames(string path, bool skipOnError)
        {
            try
            {
                return Directory.EnumerateFiles(path, "*.dll").Concat(Directory.EnumerateFiles(path, "*.exe"));
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is DirectoryNotFoundException || e is IOException || e is SecurityException || e is UnauthorizedAccessException)))
                {
                    throw;
                }

                return new string[0];
            }
        }

        private static Assembly LoadAssembly(string assemblyName, bool skipOnError)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)))
                {
                    throw;
                }

                return null;
            }
        }

        private static IEnumerable<Assembly> GetLoadedAssemblies(bool includeSystemAssemblies, bool includeUnityAssemblies, bool includeDynamicAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (includeSystemAssemblies && includeDynamicAssemblies)
            {
                return assemblies;
            }

            return assemblies.Where(a => (includeDynamicAssemblies || !a.IsDynamic) && (includeSystemAssemblies || !IsSystemAssembly(a)) && (includeUnityAssemblies || !IsUnityAssembly(a)));
        }

        public static IEnumerable<Type> FromAssemblies(params Assembly[] assemblies)
        {
            return FromAssemblies(true, assemblies);
        }

        public static IEnumerable<Type> FromAssemblies(bool skipOnError, params Assembly[] assemblies)
        {
            return FromAssemblies(assemblies, skipOnError);
        }

        public static IEnumerable<Type> FromAssemblies(IEnumerable<Assembly> assemblies, bool skipOnError = true)
        {
            Guard.ArgumentNotNull(assemblies, "assemblies");

            return FromCheckedAssemblies(CheckAssemblies(assemblies), skipOnError);
        }

        private static IEnumerable<Type> FromCheckedAssemblies(IEnumerable<Assembly> assemblies, bool skipOnError)
        {
            return assemblies
                    .SelectMany(a =>
                    {
                        IEnumerable<Type> types;

                        try
                        {
                            types = a.GetTypes();
                        }
                        catch (ReflectionTypeLoadException e)
                        {
                            if (!skipOnError)
                            {
                                throw;
                            }

                            types = e.Types.TakeWhile(t => t != null);
                        }

                        return types.Where(ti => ti.IsClass & !ti.IsAbstract && !ti.IsValueType && ti.IsVisible);
                    });
        }

        private static IEnumerable<Assembly> CheckAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    throw new ArgumentException("Null", "assemblies");
                }
            }

            return assemblies;
        }
    }
}
