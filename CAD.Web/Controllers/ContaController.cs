using CAD.Web.Model;
using System.Configuration;
using System.Web.Mvc;

namespace CAD.Web.Controllers
{
    public class ContaController : Controller
    {
        private readonly IServicoCADMembership _membership;
        private readonly IConfigurationReader _configurationReader;
        private readonly RepositorioTempData _repositorioTempData;
        private const string ReturnUrl = "ReturnUrl";

        public ContaController(IServicoCADMembership membership, IConfigurationReader configurationReader)
        {
            _membership = membership;
            _configurationReader = configurationReader;
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

            _membership.Autenticar(model.Login, model.Senha);

            var returnUrl = _repositorioTempData.Buscar<string>(ReturnUrl);
            return Redirect(returnUrl ?? _configurationReader.GetAppSetting(ReturnUrl));
        }

        [HttpGet, Authorize]
        public ActionResult AreaAutorizada()
        {
            return View();
        }
    }

    public interface IConfigurationReader
    {
        string GetAppSetting(string key);
    }

    public class ConfigurationReader : IConfigurationReader
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }

    public interface IServicoCADMembership
    {
        void Autenticar(string login, string senha);
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