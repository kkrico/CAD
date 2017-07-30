using System.Web.Optimization;

namespace CAD.Web
{
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