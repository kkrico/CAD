using Cad.Core.Negocio.Exception;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAD.Web.Infraestrutura.Seguranca.Filters
{
    public class GlobalErrorHandler : HandleErrorAttribute
    {
        private const string ViewDataKey = "ViewData";
        private const string RotaOrigemKey = "RotaOrigem";

        public override void OnException(ExceptionContext filterContext)
        {
            var excecao = filterContext.Exception;

            if (excecao is NegocioException)
            {
                TratarErrosNegocio(excecao as NegocioException, filterContext);

                TratarApresentacao(filterContext);

            }
            else
            {
                filterContext.ExceptionHandled = false;
            }
        }

        #region Tratamento Apresentacao

        private void TratarApresentacao(ExceptionContext filterContext)
        {
            var rotaOrigem = filterContext.HttpContext.Request.UrlReferrer.GetRouteData();

            filterContext.HttpContext.Response.Clear();

            filterContext.Controller.TempData[ViewDataKey] = filterContext.Controller.ViewData;
            filterContext.Controller.TempData[RotaOrigemKey] = $"{rotaOrigem["controller"]}/{rotaOrigem["action"]}";

            filterContext.Result = new RedirectToRouteResult(rotaOrigem);
        }

        #endregion

        # region Tratamento Negocio

        private static void TratarErrosNegocio(NegocioException excecao, ExceptionContext contexto)
        {
            AddErro(excecao.Message, contexto);

            contexto.ExceptionHandled = true;
        }


        private static void AddErro(string mensagem, ExceptionContext contexto)
        {
            contexto.Controller.ViewData.ModelState.AddModelError(string.Empty, mensagem);
        }

        #endregion
    }

    public static class UriExtensions
    {
        public static RouteValueDictionary GetRouteData(this Uri uri)
        {
            // Split the url to url + query string
            var fullUrl = uri.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;
            if (questionMarkIndex != -1) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }

            // Arranges
            var request = new HttpRequest(null, url, queryString);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);

            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            // Extract the data    
            var values = routeData.Values;
            var controllerName = values["controller"];
            var actionName = values["action"];
            var areaName = values["area"];

            return new RouteValueDictionary(new
            {
                controller = controllerName,
                action = actionName,
                area = areaName
            });
        }
    }

    public class RestaurarViewDataAposExcecaoAttribute : ActionFilterAttribute
    {
        private readonly string ViewDataKey = "ViewData";
        private readonly string RotaOrigemKey = "RotaOrigem";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var tempData = filterContext.Controller.TempData;

            var rotaAtual =
                $"{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{filterContext.ActionDescriptor.ActionName}";

            var possuiViewData = tempData.ContainsKey(ViewDataKey);
            var ehActionDeRetorno = string.Compare(rotaAtual, (string)tempData[RotaOrigemKey], StringComparison.OrdinalIgnoreCase);

            if (possuiViewData && ehActionDeRetorno == 0)
            {
                filterContext.Controller.ViewData = (ViewDataDictionary)tempData[ViewDataKey];
            }
        }
    }
}