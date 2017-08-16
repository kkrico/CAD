using Cad.Core.Dados;
using Cad.Core.Dados.Entidades;
using Cad.Core.Dados.Repositorio;
using Cad.Core.Negocio.DTO;
using Cad.Core.Negocio.Exception;
using Cad.Core.Negocio.Recursos;
using Cad.Core.Negocio.Servico.Interface;
using Cad.Core.Util.Guard;
using System;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Security;

namespace Cad.Core.Negocio.Servico
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly RepositorioAtualizavel<Usuario> _repositorioUsuario;
        private readonly EmailServico _servicoEmail;

        public UsuarioServico(CADContext context)
        {
            _repositorioUsuario = new RepositorioAtualizavel<Usuario>(context);
            _servicoEmail = new EmailServico();
        }

        public void Autenticar(UsuarioDTO usuario)
        {
            Guard.IsNotNull(usuario);

            var crypSenha = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.Senha, FormsAuthPasswordFormat.SHA1.ToString());

            var usuarioEncontrado = _repositorioUsuario.Existe(u => u.Senha == crypSenha && u.Login == usuario.Login);

            if (!usuarioEncontrado)
                throw new NegocioException("Usuário inválido ou não encontrado");

            FormsAuthentication.SetAuthCookie(usuario.Login, usuario.LembrarSenha);
        }

        public void SolicitarMudancaSenha(UsuarioNovaSenhaDTO usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            var usuarioEncontrado = _repositorioUsuario.Obter(u => u.Login == usuario.Login);
            if (usuarioEncontrado == null) return;

            usuarioEncontrado.HasAlteracaoSenha = true;
            _repositorioUsuario.Atualizar(usuarioEncontrado);
            _repositorioUsuario.SalvarAlteracoes();


            var d = new DestinatarioMensagemDTO()
            {
                Nome = usuarioEncontrado.Login,
                Email = usuario.Email
            };
            var mensagem = _servicoEmail.ObterMensagemAlteracaoSenha(d);
            _servicoEmail.EnviarMensagem(mensagem);
        }
    }

    public class DestinatarioMensagemDTO
    {
        public string Email { get; set; }
        public string Nome { get; set; }
    }

    public class EmailServico
    {
        private readonly RemetenteMensagemDTO _remetenteMensagem;

        public EmailServico() : this(new RemetenteMensagemDTO
        {
            Email = "meajuda@cadsys.com.br",
            Nome = "Me Ajuda - CadSys"
        })
        {

        }

        public EmailServico(RemetenteMensagemDTO remetenteMensagem)
        {
            _remetenteMensagem = remetenteMensagem;
        }

        public MensagemAlteracaoSenhaDTO ObterMensagemAlteracaoSenha(DestinatarioMensagemDTO destinatario)
        {
            var msg = new MensagemAlteracaoSenhaDTO
            {
                Texto = Email.EsqueciSenha,
                Destinatario = destinatario
            };

            return msg;
        }

        public void EnviarMensagem(MensagemAlteracaoSenhaDTO mensagem)
        {
            if (mensagem == null) throw new ArgumentNullException(nameof(mensagem));

            var from = new MailAddress(_remetenteMensagem.Email, _remetenteMensagem.Nome);
            var to = new MailAddress(mensagem.Destinatario.Email, mensagem.Destinatario.Nome);
            var mail = new MailMessage(from, to);
            var client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "localhost"
            };
            mail.Subject = mensagem.Assunto;
            mail.Body = mensagem.Texto;
            mail.IsBodyHtml = true;
            client.Send(mail);

        }
    }

    public class RemetenteMensagemDTO
    {
        public string Email { get; set; }
        public string Nome { get; set; }
    }

    public class MensagemAlteracaoSenhaDTO
    {
        public DestinatarioMensagemDTO Destinatario { get; set; }
        public string Texto { get; set; }

        public string Assunto => "Alteração de Email - CADSYS";
    }

    public class UsuarioNovaSenhaDTO
    {
        public string Login { get; set; }
        public string Email { get; set; }
    }
}