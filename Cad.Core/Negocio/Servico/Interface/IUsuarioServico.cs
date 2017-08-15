using Cad.Core.Negocio.DTO;

namespace Cad.Core.Negocio.Servico.Interface
{
    public interface IUsuarioServico
    {
        void Autenticar(UsuarioDTO usuario);
        void SolicitarMudancaSenha(UsuarioNovaSenhaDTO dto);
    }
}