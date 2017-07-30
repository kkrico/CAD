using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAD.Web.Infraestrutura.IOC
{
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IUnityContainer _container;

        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var list = base.GetActionAttributes(controllerContext, actionDescriptor);

            foreach (var item in list)
            {
                this._container.BuildUp(item.GetType(), item);
            }

            return list;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var list = base.GetControllerAttributes(controllerContext, actionDescriptor);

            foreach (var item in list)
            {
                this._container.BuildUp(item.GetType(), item);
            }

            return list;
        }
    }
}