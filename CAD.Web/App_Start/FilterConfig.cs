using CAD.Web.Infraestrutura.Seguranca.Filters;
using System.Web.Mvc;

namespace CAD.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalErrorHandler());
            filters.Add(new RestaurarViewDataAposExcecaoAttribute());
        }
    }
}