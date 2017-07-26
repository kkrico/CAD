using Cad.Core.Negocio.Mensagem;
using Cad.Core.Negocio.Servico.Interface;
using Cad.Test.Util;
using CAD.Web.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CAD.Web.Controllers.Test
{
    [TestClass]
    public class ContaControllerTests
    {
        [TestMethod]
        [TestCategory("Controllers")]
        public void Login_Retorna_View()
        {
            var ctr = GetContaController();
            var view = ctr.Login() as ViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Login_Lanca_Excecao()
        {
            var ctr = GetContaController();
            var view = ctr.Login(new LoginVM()) as ViewResult;
        }

        private static ContaController GetContaController(Mock<IServicoConta> mockServicoConta = null)
        {
            if (mockServicoConta == null)
                mockServicoConta = new Mock<IServicoConta>();

            var ctrl = new ContaController(mockServicoConta.Object)
            {
                ControllerContext = new ControllerContext()
            };

            return ctrl;
        }

        [TestMethod]
        public void LoginController_DeveTer_LoginObrigatorio()
        {
            var mockServicoConta = new Mock<IServicoConta>();
            var ctrl = GetContaController(mockServicoConta);

            var form = new FormCollection
            {
                { nameof(LoginVM.Senha), "0123456789012345678901234567890123456789" },
                { nameof(LoginVM.ConfirmacaoSenha), "0123456789012345678901234567890123456789" },
            };


            var model = ModelBinderUtil.BindModel<LoginVM>(ctrl, form);

            var view = ctrl.Login(model) as ViewResult;

            Assert.AreEqual(Mensagem.M003, ctrl.ModelState["Login"].Errors.First().ErrorMessage);
            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }

        [TestMethod]
        public void Login_DeveSer_CPF()
        {
            var ctr = GetContaController();
            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "email@email.com.br" },
                { nameof(LoginVM.Senha), "0123456789012345678901234567890123456789" },
                { nameof(LoginVM.ConfirmacaoSenha), "0123456789012345678901234567890123456789" },
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            var view = ctr.Login(model) as ViewResult;

            Assert.AreEqual(Mensagem.M011, ctr.ModelState["Login"].Errors.First().ErrorMessage);
            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }
    }
}