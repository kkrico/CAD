using Cad.Web;
using CAD.Web.Infraestructure.IOC;
using System.Web.Mvc;

namespace CAD.Web
{
    public class IOC
    {
        public static void ConfigureContainer(ControllerBuilder current)
        {
            RegisterContainer(current);

            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.GetConfiguredContainer()));
        }

        private static void RegisterContainer(ControllerBuilder current)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var controllerFactory = new UnityControllerFactory(container);
            current.SetControllerFactory(controllerFactory);
        }

        public static void ShutdownContainers()
        {
            var container = UnityConfig.GetConfiguredContainer();

            container.Dispose();
        }
    }
}