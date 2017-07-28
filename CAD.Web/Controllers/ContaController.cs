using Cad.Core.Negocio.Exception;
using Cad.Core.Negocio.Mensagem;
using Cad.Core.Negocio.Servico.Interface;
using CAD.Web.Model;
using System.Web.Mvc;
using System.Web.Security;

namespace CAD.Web.Controllers
{
    public class ContaController : Controller
    {
        private readonly IServicoUsuario _servicoUsuario;
        private readonly RepositorioTempData _repositorioTempData;
        private const string ReturnUrl = "ReturnUrl";

        public ContaController(IServicoUsuario servicoUsuario)
        {
            _servicoUsuario = servicoUsuario;
            _repositorioTempData = new RepositorioTempData(TempData);
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

            var isValido = Membership.ValidateUser(model.Login, model.Senha);

            if (!isValido) throw new NegocioException(Mensagem.M001);

            var returnUrl = _repositorioTempData.Buscar<string>(ReturnUrl);
            return Redirect(returnUrl ?? "http://google.com.br");
        }

        [HttpGet, Authorize]
        public ActionResult AreaAutorizada()
        {
            return View();
        }
    }

    public class RepositorioTempData
    {
        private readonly TempDataDictionary _tempData;
        public RepositorioTempData(TempDataDictionary tempData)
        {
            _tempData = tempData;
        }
        public void Adicionar(string key, object valor)
        {
            _tempData[key] = valor;
        }

        public void Excluir(string key)
        {
            _tempData.Remove(key);
        }

        public object Buscar(string key)
        {
            var valor = _tempData[key];
            _tempData.Keep(key);
            return valor;
        }

        public T Buscar<T>(string key)
        {
            var valor = _tempData[key];
            _tempData.Keep(key);
            return (T)valor;
        }
    }
}