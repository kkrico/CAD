using Cad.Web;
using CAD.Web.Infraestructure;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAD.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            IOC.ConfigureContainer(ControllerBuilder.Current);


            var container = UnityConfig.GetConfiguredContainer();

            var erro = container.Resolve<IRunOnError>();
            var t = container.ResolveAll<IRunOnError>().ToList();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var erro = container.Resolve<IRunOnError>();
            var t = container.ResolveAll<IRunOnError>().ToList();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var erro = container.Resolve<IRunOnError>();
            var t = container.ResolveAll<IRunOnError>().ToList();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            IOC.ShutdownContainers();

        }
    }
}