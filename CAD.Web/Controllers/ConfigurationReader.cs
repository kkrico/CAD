using System.Configuration;
using CAD.Web.Infraestrutura.Seguranca.Interfaces;

namespace CAD.Web.Controllers
{
    public class ConfigurationReader : IConfigurationReader
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }
}