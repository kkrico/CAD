using Cad.Core.Negocio.DTO;
using Cad.Core.Negocio.Servico.Interface;
using CAD.Web.Infraestructure;
using CAD.Web.Model;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace CAD.Web.Controllers
{
    public class ContaController : Controller
    {
        private readonly IServicoConta _servicoConta;

        public ContaController(IServicoConta servicoConta)
        {
            _servicoConta = servicoConta;
        }

        [HttpGet]
        [DemoFilter]
        public ActionResult Login()
        {
            
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View("Login");

            throw new NotImplementedException();
        }
    }

    public class DemoFilterAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IRunOnError RunOnError { get; set; }
        [Dependency]
        public IServicoConta ServicoConta { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ServicoConta.Autenticar(new UsuarioDTO());
            base.OnActionExecuted(filterContext);
        }
    }
}