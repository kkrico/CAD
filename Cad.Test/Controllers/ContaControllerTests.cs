using Cad.Core.Negocio.Exception;
using Cad.Core.Negocio.Mensagem;
using Cad.Test.Util;
using CAD.Web.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Web.Mvc;

namespace CAD.Web.Controllers.Test
{
    [TestClass]
    public class ContaControllerTests
    {
        private const string ReturnUrl = "ReturnUrl";
        private const string ReturnedUrl = "http://google.com.br";

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
        public void Login_Com_Usuario_Senha()
        {
            var mockMember = new Mock<IServicoCADMembership>();
            mockMember.Setup(s => s.Autenticar("01614865175", "123456")).Throws(new NegocioException(Mensagem.M001));

            var ctr = GetContaController(mockMember);
            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "68880928104" },
                { nameof(LoginVM.Senha), "123456" },
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            var res = ctr.Login(model) as RedirectResult;

            Assert.IsNotNull(res);
            Assert.AreEqual(res.Url, ReturnedUrl);
            Assert.IsFalse(res.Permanent);
        }

        [TestMethod]
        public void LoginController_DeveTer_LoginObrigatorio()
        {
            var ctrl = GetContaController();

            var form = new FormCollection
            {
                { nameof(LoginVM.Senha), "0123456789012345678901234567890123456789" },
            };


            var model = ModelBinderUtil.BindModel<LoginVM>(ctrl, form);

            var view = ctrl.Login(model) as ViewResult;

            Assert.IsTrue(ctrl.ModelState["Login"].Errors.Any(m => m.ErrorMessage == Mensagem.M003));
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
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            var view = ctr.Login(model) as ViewResult;

            Assert.AreEqual(Mensagem.M011, ctr.ModelState["Login"].Errors.First().ErrorMessage);
            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }

        private static ContaController GetContaController(Mock<IServicoCADMembership> mockMembership = null)
        {
            if (mockMembership == null)
                mockMembership = new Mock<IServicoCADMembership>();

            var mockReader = new Mock<IConfigurationReader>();
            mockReader.Setup(x => x.GetAppSetting(ReturnUrl)).Returns(ReturnedUrl);

            var ctrl = new ContaController(mockMembership.Object, mockReader.Object)
            {
                ControllerContext = new ControllerContext()
            };

            return ctrl;
        }
    }
}