using Cad.Core.Negocio.DTO;

namespace Cad.Core.Negocio.Servico.Interface
{
    public interface IServicoConta
    {
        void Autenticar(UsuarioDTO usuario);
    }
}