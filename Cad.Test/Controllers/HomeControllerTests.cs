using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace CAD.Web.Controllers.Test
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        [TestCategory("Controllers")]
        public void Index_Retorna_View()
        {

            var ctr = new HomeController();

            var view = ctr.Index() as ViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("Index", view.ViewName);
        }
    }
}