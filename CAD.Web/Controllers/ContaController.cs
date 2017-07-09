using Cad.Core.Negocio.Servico.Interface;
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

    public class LoginVM
    {
    }
}