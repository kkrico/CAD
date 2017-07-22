using Cad.Core.Negocio.Servico.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Web.Mvc;
using CAD.Web.Model;

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

        private static ContaController GetContaController()
        {
            var servicoConta = new Mock<IServicoConta>();

            var ctr = new ContaController(servicoConta.Object);
            return ctr;
        }
    }
}