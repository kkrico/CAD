using System.Web.Mvc;

namespace Cad.Test.Util
{
    public static class ModelBinderUtil
    {
        public static TModel BindModel<TModel>(Controller controller, IValueProvider valueProvider) where TModel : class
        {
            var binder = ModelBinders.Binders.GetBinder(typeof(TModel));
            var bindingContext = new ModelBindingContext()
            {
                FallbackToEmptyPrefix = true,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModel)),
                ModelName = "NotUsedButNotNull",
                ModelState = controller.ModelState,
                PropertyFilter = (name => true),
                ValueProvider = valueProvider
            };

            return (TModel)binder.BindModel(controller.ControllerContext, bindingContext);
        }
    }
}