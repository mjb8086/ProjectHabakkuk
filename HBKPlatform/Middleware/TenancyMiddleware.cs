using HBKPlatform.Services;

namespace HBKPlatform.Middleware
{
    public class TenancyMiddleware(ITenancyService _tenancySrv) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var c = context.User.FindFirst("TenancyId");
            if (c != null && int.TryParse(c.Value, out int val))
            {
                _tenancySrv.SetTenancyId(val);
            }
            else
            {
                _tenancySrv.SetTenancyId(-1);
            }
            await next(context);
        }
    }
}