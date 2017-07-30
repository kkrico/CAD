using System.Web.Mvc;

namespace CAD.Web.Infraestrutura.MVC
{
    public interface IRepositorioTempData
    {
        void Adicionar(string key, object valor);
        void Excluir(string key);
        object Buscar(string key);
        T Buscar<T>(string key);
    }

    public class RepositorioTempData : IRepositorioTempData
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