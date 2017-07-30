namespace CAD.Web.Infraestrutura.Seguranca.Interfaces
{
    public interface IConfigurationReader
    {
        string GetAppSetting(string key);
    }
}