using Cad.Web;
using CAD.Web.Infraestrutura.IOC;
using System.Web.Mvc;

namespace CAD.Web
{
    public class IOCConfig
    {
        public static void RegisterContainers(ControllerBuilder current)
        {
            RegisterContainer(current);
        }

        public static void RegisterFilters()
        {
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.GetConfiguredContainer()));
        }

        private static void RegisterContainer(ControllerBuilder current)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var controllerFactory = new UnityControllerFactory(container);
            current.SetControllerFactory(controllerFactory);
        }

        public static void DisposeContainers()
        {
            var container = UnityConfig.GetConfiguredContainer();

            container.Dispose();
        }
    }
}