using Cad.Core.Negocio.DTO;

namespace Cad.Core.Negocio.Servico.Interface
{
    public interface IServicoUsuario
    {
        void Autenticar(UsuarioDTO usuario);
    }
}