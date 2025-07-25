using Microsoft.AspNetCore.Authentication;

namespace PRASARNET.Services
{
    public class SidLogoutCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public SidLogoutCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISidStore sidStore)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var sid = context.User.FindFirst("sid")?.Value;
                if (!string.IsNullOrEmpty(sid))
                {
                    bool isInvalid = await sidStore.IsSidInvalidatedAsync(sid);
                    if (isInvalid)
                    {
                        await context.SignOutAsync("Cookies");
                        await context.SignOutAsync("oidc");

                        context.Response.Redirect("/Account/Index"); // or login page
                        return;
                    }
                } 
            }

            await _next(context);
        }
    }

}
