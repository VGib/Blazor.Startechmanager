using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Text;

namespace Blazor.Startechmanager.Server
{
    public static class ModelStateExtension
    {
        public static  string GetNonValidationErrorMessage(this ModelStateDictionary modelState)
        {
            var stringBuilder = new StringBuilder("some error Occured in validation" + Environment.NewLine);
            stringBuilder.AppendLine();
            foreach(var valuesWithErrors in modelState.Values.Where(x => x.Errors.Count > 0))
            {
                stringBuilder.AppendLine($"### Error for {valuesWithErrors.AttemptedValue} = {valuesWithErrors.RawValue}");
                
                foreach(var error in valuesWithErrors.Errors)
                {
                    stringBuilder.AppendLine($"     -->  {error.ErrorMessage}");
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
