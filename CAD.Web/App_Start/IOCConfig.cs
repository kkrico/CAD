using Cad.Web;
using CAD.Web.Infraestructure;
using System.Web.Mvc;

namespace CAD.Web
{
    public class IOCConfig
    {
        public static void StartContainers(ControllerBuilder current)
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