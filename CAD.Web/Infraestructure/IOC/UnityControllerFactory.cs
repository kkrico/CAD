using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAD.Web.Infraestructure.IOC
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            this._container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return _container.Resolve(controllerType) as IController;
        }
    }
}