using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Utilidades
{
    public class TypeBinder<T>:IModelBinder
    {
   
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var valor = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if (valor == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            else
            {
                try
                {
                    var valorDeserializado = JsonConvert.DeserializeObject<T>(valor.FirstValue);
                    bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
                }
                catch
                {
                    bindingContext.ModelState.TryAddModelError
                        (nombrePropiedad, "El valor no es del tipo adecuado");
                }

                return Task.CompletedTask;
                
            }
        }
    }
}
