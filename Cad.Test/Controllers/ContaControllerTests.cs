using Cad.Core.Negocio.DTO;
using Cad.Core.Negocio.Exception;
using Cad.Core.Negocio.Mensagem;
using Cad.Core.Negocio.Servico.Interface;
using Cad.Test.Util;
using CAD.Web.Infraestrutura.Interface;
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
        public void Login_Guarda_ReturnUrl()
        {
            var mockTempData = new Mock<ITempDataServico>();
            var ctr = GetContaController(mockTempData);
            var url = "/Conta/AreaAutorizada";
            var view = ctr.Login(url) as ViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
            mockTempData.Verify(x => x.Adicionar(ReturnUrl, url), Times.Once());
        }

        [TestMethod]
        [TestCategory("Controllers")]
        public void Login_DeveSer_Obrigatorio()
        {
            var ctrl = GetContaController();

            var form = new FormCollection
            {
                { nameof(LoginVM.Senha), "0123456789012345678901234567890123456789" },
            };


            var model = ModelBinderUtil.BindModel<LoginVM>(ctrl, form);

            var view = ctrl.Login(model) as ViewResult;

            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsTrue(ctrl.ModelState["Login"].Errors.Any(m => m.ErrorMessage == Mensagem.M003));
            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        public void Senha_DeveSer_Obrigatorio()
        {
            var ctrl = GetContaController();

            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "01614865175" },
            };


            var model = ModelBinderUtil.BindModel<LoginVM>(ctrl, form);

            var view = ctrl.Login(model) as ViewResult;

            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsTrue(ctrl.ModelState["Senha"].Errors.Any(m => m.ErrorMessage == Mensagem.M003));
            Assert.IsNotNull(view);
            Assert.AreEqual("Login", view.ViewName);
        }

        [TestMethod]
        [TestCategory("Controllers")]
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

        [TestMethod]
        [TestCategory("Controllers")]
        [ExpectedException(typeof(NegocioException))]
        public void Login_Invalido_Lanca_Excecao()
        {
            var mockServicoUsuario = new Mock<IUsuarioServico>();
            mockServicoUsuario.Setup(x => x.Autenticar(It.IsAny<UsuarioDTO>()))
                .Throws(new NegocioException(Mensagem.M001));
            var ctr = GetContaController(mockServicoUsuario);
            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "01614865175" },
                { nameof(LoginVM.Senha), "1234567890" },
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            ctr.Login(model);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        public void Login_Valido_Redireciona_DeVoltaPadrao_Se_NaoInformar_URÇ()
        {
            var mockServicoUsuario = new Mock<IUsuarioServico>();
            var ctr = GetContaController(mockServicoUsuario);
            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "01614865175" },
                { nameof(LoginVM.Senha), "1234567890" },
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            var view = ctr.Login(model) as RedirectResult;

            mockServicoUsuario.Verify(m => m.Autenticar(It.IsAny<UsuarioDTO>()), Times.Once());
            Assert.IsNotNull(view);
            Assert.IsTrue(view.Url == ReturnedUrl);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        public void Login_Valido_Redireciona_Url_Temp_Data()
        {
            const string urlTempData = "/home/arearestrita";
            var mockServicoUsuario = new Mock<IUsuarioServico>();
            var mockTempData = new Mock<ITempDataServico>();
            mockTempData.Setup(x => x.Buscar<string>(ReturnUrl)).Returns(urlTempData);
            var ctr = GetContaController(mockServicoUsuario, mockTempData);
            var form = new FormCollection
            {
                { nameof(LoginVM.Login), "01614865175" },
                { nameof(LoginVM.Senha), "1234567890" },
            };

            var model = ModelBinderUtil.BindModel<LoginVM>(ctr, form);
            var view = ctr.Login(model) as RedirectResult;

            mockServicoUsuario.Verify(m => m.Autenticar(It.IsAny<UsuarioDTO>()), Times.Once());
            Assert.IsNotNull(view);
            Assert.IsTrue(view.Url == urlTempData);
        }

        private static ContaController GetContaController()
        {
            var mockTempData = new Mock<ITempDataServico>();
            var mockServicoUsuario = new Mock<IUsuarioServico>();

            return GetContaController(mockServicoUsuario, mockTempData);
        }

        private static ContaController GetContaController(Mock<ITempDataServico> tempMock)
        {
            return GetContaController(new Mock<IUsuarioServico>(), tempMock);
        }

        private static ContaController GetContaController(Mock<IUsuarioServico> servicoUsuario)
        {
            return GetContaController(servicoUsuario, new Mock<ITempDataServico>());
        }

        private static ContaController GetContaController(Mock<IUsuarioServico> servicoUsuario, Mock<ITempDataServico> mockTempData)
        {
            if (servicoUsuario == null)
                servicoUsuario = new Mock<IUsuarioServico>();

            if (mockTempData == null)
                mockTempData = new Mock<ITempDataServico>();

            var mockReader = new Mock<IConfigurationReader>();
            mockReader.Setup(x => x.GetAppSetting(ReturnUrl)).Returns(ReturnedUrl);


            var ctrl = new ContaController(mockReader.Object, servicoUsuario.Object, mockTempData.Object)
            {
                ControllerContext = new ControllerContext()
            };

            return ctrl;
        }
    }
}