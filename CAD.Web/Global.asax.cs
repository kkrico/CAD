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
            IOCConfig.RegisterContainers(ControllerBuilder.Current);
            IOCConfig.RegisterFilters();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            IOCConfig.DisposeContainers();
        }
    }
}