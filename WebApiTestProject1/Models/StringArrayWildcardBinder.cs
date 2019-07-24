using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace WebApiTestProject1.Models
{
    public class StringArrayWildcardBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);

            if (val != null)
            {
                var s = val.AttemptedValue;

                if (s != null)
                {
                    try
                    {
                        // Parse the elements on the forward slash
                        var array = s.Split('/');
                        bindingContext.Model = array;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}