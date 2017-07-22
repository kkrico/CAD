using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAD.Web.Infraestructure.IOC
{
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IUnityContainer container;

        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            this.container = container;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var list = base.GetActionAttributes(controllerContext, actionDescriptor);

            foreach (var item in list)
            {
                this.container.BuildUp(item.GetType(), item);
            }

            return list;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var list = base.GetControllerAttributes(controllerContext, actionDescriptor);

            foreach (var item in list)
            {
                this.container.BuildUp(item.GetType(), item);
            }

            return list;
        }
    }
}