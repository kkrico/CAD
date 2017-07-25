using System;
using System.Web.Mvc;
using System.Web.Optimization;
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
            BundleConfig.RegisterBundes(BundleTable.Bundles);
            IOC.ConfigureContainer(ControllerBuilder.Current);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            IOC.ShutdownContainers();
        }
    }

    public class BundleConfig
    {
        public static void RegisterBundes(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                                    .Include("~/Content/bootstrap.css")
                                    .Include("~/Content/CAD/cad.css"));

            bundles.Add(new ScriptBundle("~/js")
                                    .Include("~/Scripts/jquery-{version}.js")
                                    .Include("~/Scripts/bootstrap.js"));
        }
    }
}