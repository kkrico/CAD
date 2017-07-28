using Cad.Core.Dados;
using Cad.Core.Dados.Entidades;
using Cad.Core.Dados.Repositorio;
using Cad.Core.Negocio.DTO;
using Cad.Core.Negocio.Exception;
using Cad.Core.Negocio.Servico.Interface;
using Cad.Core.Util.Guard;
using System.Web.Configuration;
using System.Web.Security;

namespace Cad.Core.Negocio.Servico
{
    public class ServicoUsuario : IServicoUsuario
    {
        private readonly Repositorio<Usuario> _repositorioUsuario;

        public ServicoUsuario(CADContext context)
        {
            _repositorioUsuario = new Repositorio<Usuario>(context);
        }

        public void Autenticar(UsuarioDTO usuario)
        {
            Guard.IsNotNull(usuario);

            var crypSenha = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.Senha, FormsAuthPasswordFormat.SHA1.ToString());
            
            var usuarioEncontrado = _repositorioUsuario.Existe(u => u.Senha ==  crypSenha && u.Login == usuario.Login);

            if (!usuarioEncontrado)
                throw new NegocioException("Usuário inválido ou não encontrado");
        }
    }
}