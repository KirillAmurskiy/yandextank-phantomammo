using Microsoft.AspNetCore.Builder;

namespace YadnexTank.PhantomAmmo.AspNetCore
{
    public static class PhantomAmmoCollectorMiddlewareExtensions
    {
        public static IApplicationBuilder UsePhantomAmmoCollector(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PhantomAmmoCollectorMiddleware>();
        }
    }
}