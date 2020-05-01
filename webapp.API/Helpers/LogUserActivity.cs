using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using webapp.API.Data;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace webapp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); //wait until the action compliated

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IFriendshipRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}