using Cad.Core.Negocio.Servico.Interface;
using CAD.Web.Infraestrutura.MVC;
using CAD.Web.Infraestrutura.Seguranca.Interfaces;
using CAD.Web.Model;
using System.Web.Mvc;

namespace CAD.Web.Controllers
{
    public class ContaController : Controller
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IServicoUsuario _servicoUsuario;
        private readonly IRepositorioTempData _repositorioTempData;
        private const string ReturnUrl = "ReturnUrl";

        public ContaController(IConfigurationReader configurationReader, IServicoUsuario servicoUsuario, IRepositorioTempData tempData)
        {
            _configurationReader = configurationReader;
            _servicoUsuario = servicoUsuario;
            _repositorioTempData = tempData;
        }

        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            _repositorioTempData.Adicionar(ReturnUrl, returnUrl);
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View("Login");

            var dto = LoginVM.Converter(model);
            _servicoUsuario.Autenticar(dto);

            var returnUrl = _repositorioTempData.Buscar<string>(ReturnUrl);
            return Redirect(string.IsNullOrEmpty(returnUrl) ? _configurationReader.GetAppSetting(ReturnUrl) : returnUrl);
        }

        [HttpGet, Authorize]
        public ActionResult AreaAutorizada()
        {
            return View();
        }
    }
}