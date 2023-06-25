using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    public static class ModelStateExtension
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState) 
            => modelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage)).ToList();
    }
}
