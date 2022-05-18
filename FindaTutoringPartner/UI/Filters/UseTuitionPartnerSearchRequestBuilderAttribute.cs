using Microsoft.AspNetCore.Mvc.Filters;

namespace UI.Filters;

public class UseTuitionPartnerSearchRequestBuilderAttribute : ActionFilterAttribute
{
    private const string SearchId = "search_id";

    //private IActionFilter

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Guid searchId;

        if (!context.RouteData.Values.TryGetValue(SearchId, out var searchIdValue) || searchIdValue is not Guid)
        {
            searchId = Guid.NewGuid();
        }
        else
        {
            searchId = (Guid)searchIdValue;
        }

        //var builder = null;

        context.ActionArguments.Add("builder", null);

        // Do something before the action executes.
        await next();

        if (context.ModelState.IsValid)
        {

        }

        //context.HttpContext.Response.

        // Do something after the action executes.
    }
}