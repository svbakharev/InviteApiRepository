using InviteApiContract.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace InviteApi.Fiters
{
    public class InviteCommandFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var firstError = context.ModelState
                    .SelectMany(x => x.Value.Errors)
                    .First();

                context.Result = new ObjectResult(firstError.ErrorMessage)
                {
                    StatusCode = InviteErrors.GetErrorCode(firstError.ErrorMessage)
                };
            }
        }        
    }
}
