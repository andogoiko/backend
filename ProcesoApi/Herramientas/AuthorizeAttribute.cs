using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using BDD;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AutohorrizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User)context.HttpContext.Items["User"];
        if (user == null)
        {
            // no logeado
            context.Result = new JsonResult(
                new { 
                    message = "Unauthorrorized" 
                }) 
                { 
                    StatusCode = StatusCodes.Status401Unauthorized 
                };
        }
    }
}