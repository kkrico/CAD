using Cad.Core.Dados;
using CAD.Web.Infraestrutura.Interface;
using CAD.Web.Infraestrutura.IOC;
using CAD.Web.Infraestrutura.MVC;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ITempDataServico, TempDataServico>(new ContainerControlledLifetimeManager());
            container.RegisterType<CADContext>(new HierarchicalLifetimeManager(), new InjectionConstructor());
        }

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
                        container.RegisterType(typeMapping.Key, type, GetNameForRegistration(type));
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

        private static string GetNameForRegistration(Type type)
        {
            var name = type.Name;

            return name;
        }

        private static IEnumerable<Type> GetClassesFromAssemblies(IEnumerable<Assembly> assemblies = null)
        {
            var allClasses = assemblies != null ? AllClasses.FromAssemblies(assemblies) : AllClasses.FromAssembliesInBasePath();
            return
                allClasses.Where(
                    n =>
                        n.Namespace != null);
        }
    }
}
