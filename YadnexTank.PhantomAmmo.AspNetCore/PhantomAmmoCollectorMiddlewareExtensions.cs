using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YadnexTank.PhantomAmmo.AspNetCore
{
    public static class PhantomAmmoCollectorMiddlewareExtensions
    {
        public static IServiceCollection AddPhantomAmmoCollector(
            this IServiceCollection services,
            Action<PhantomAmmoCollectorOptions> setupAction = null)
        {
            if (setupAction != null)
            {
                services
                    .AddOptions<PhantomAmmoCollectorOptions>()
                    .Configure(setupAction);    
            }
            return services;
        }
        
        public static IServiceCollection AddPhantomAmmoCollector(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<PhantomAmmoCollectorOptions>(configuration);
            return services;
        }
        
        public static IServiceCollection AddPhantomAmmoCollector(
            this IServiceCollection services,
            Action<PhantomAmmoCollectorOptions> setupAction,
            IConfiguration configuration)
        {
            services
                .AddOptions<PhantomAmmoCollectorOptions>()
                .Configure(setupAction);
            services.Configure<PhantomAmmoCollectorOptions>(configuration);
            return services;
        }

        public static IApplicationBuilder UsePhantomAmmoCollector(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PhantomAmmoCollectorMiddleware>();
        }
    }
}