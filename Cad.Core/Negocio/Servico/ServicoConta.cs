using Cad.Core.Negocio.DTO;
using Cad.Core.Negocio.Servico.Interface;
using System.Diagnostics;

namespace Cad.Core.Negocio.Servico
{
    public class ServicoConta : IServicoConta
    {
        public void Autenticar(UsuarioDTO usuario)
        {
            Debug.Write("Servico Conta!!!");
        }

        public void MetodoExclusivoDaConta()
        {
            
        }
    }
}