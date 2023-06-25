using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKey : Attribute, IAsyncActionFilter
    {
        public IConfiguration _config;

        public ApiKey(IConfiguration config)
        {
            _config = config;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) 
        {
            if(!context.HttpContext.Request.Query.TryGetValue("api_name", out var tokenName)) 
            {
                if(tokenName != Configuration.ApiKeyName)
                    context.Result = new ContentResult { StatusCode = 401, Content = "Acesso não autorizado." };
            }

            if(!context.HttpContext.Request.RouteValues.TryGetValue("api_token", out var value)) 
            {
                if(value != Configuration.ApiKey)
                    context.Result = new ContentResult { StatusCode = 403, Content = "Acesso negado." };
            }

            await next();
        }
    }
}
    