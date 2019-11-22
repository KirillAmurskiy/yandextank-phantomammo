using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using YandexTank.PhantomAmmo;

namespace YadnexTank.PhantomAmmo.AspNetCore
{
    public class PhantomAmmoCollectorMiddleware
    {
        private readonly RequestDelegate next;

        private IOptionsMonitor<PhantomAmmoCollectorOptions> optsAccessor;
        private readonly ILogger<PhantomAmmoCollectorMiddleware> logger;

        public PhantomAmmoCollectorMiddleware(
            RequestDelegate next,
            IOptionsMonitor<PhantomAmmoCollectorOptions> optsAccessor,
            ILogger<PhantomAmmoCollectorMiddleware> logger)
        {
            this.next = next;
            this.optsAccessor = optsAccessor;
            this.logger = logger ?? new NullLogger<PhantomAmmoCollectorMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var opts = optsAccessor.CurrentValue;
            if (!opts.Enabled)
            {
                await next.Invoke(context);
                return;
            }

            PhantomAmmoInfo ammo = null;
            try
            {
                ammo = await MakeAmmo(context.Request);
                await next.Invoke(context);
                await StoreAmmo(ammo, context.Response.StatusCode, opts);
            }
            catch (Exception)
            {
                await StoreAmmo(ammo, -1, opts);
                throw;
            }
        }

        
        private async Task<PhantomAmmoInfo> MakeAmmo(HttpRequest request)
        {
            try
            {
                var url = request.GetEncodedUrl();
                var ammoInfo = new PhantomAmmoInfo(url, request.Method)
                {
                    Body = await ExtractBody(request),
                    Protocol = request.Protocol
                };
                foreach (var h in request.Headers)
                {
                    ammoInfo.Headers.Add(h.Key, h.Value);
                }
                return ammoInfo;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, e.Message);
                return null;
            }
        }
        

        private async Task StoreAmmo(PhantomAmmoInfo ammo, int responseStatusCode, PhantomAmmoCollectorOptions opts)
        {
            if (ammo == null)
            {
                return;
            }

            try
            {
                ammo.Status = GetResponseStatus(responseStatusCode);
            
                if (!string.IsNullOrWhiteSpace(opts.AllRequestsFile))
                {
                    using (var file = File.AppendText(opts.AllRequestsFile))
                    {
                        await file.WriteAsync(ammo.ToString());
                    }    
                }
                if (!string.IsNullOrWhiteSpace(opts.GoodRequestsFile)
                    && ammo.Status == PhantomAmmoStatuses.Good)
                {
                    using (var file = File.AppendText(opts.GoodRequestsFile))
                    {
                        await file.WriteAsync(ammo.ToString());
                    }    
                }
                else if (!string.IsNullOrWhiteSpace(opts.BadRequestsFile))
                {
                    using (var file = File.AppendText(opts.BadRequestsFile))
                    {
                        await file.WriteAsync(ammo.ToString());
                    }    
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e, e.Message);
            }
        }

        private string GetResponseStatus(int statusCode) =>
            statusCode > 199 && statusCode < 300
                ? PhantomAmmoStatuses.Good
                : PhantomAmmoStatuses.Bad;
        
        private async Task<string> ExtractBody(HttpRequest request)
        {
            request.EnableBuffering();
            
            request.Body.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(request.Body);

            var body = await reader.ReadToEndAsync();
            
            request.Body.Seek(0, SeekOrigin.Begin);
            
            return body;
        }
    }
}