using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Seiun.Models.Responses;

namespace Seiun.Filters;

public class ParamValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0).Select(e => new ParamValidationError
        {
            Field = e.Key,
            Errors = e.Value?.Errors.Select(err => err.ErrorMessage)
        });

        context.Result = new BadRequestObjectResult(new ParamValidationResp(errors));
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}