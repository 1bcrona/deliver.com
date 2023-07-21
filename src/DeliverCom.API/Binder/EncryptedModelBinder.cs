using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliverCom.API.Binder
{
    public class EncryptedModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue;
            var model = value.Split(",");
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}