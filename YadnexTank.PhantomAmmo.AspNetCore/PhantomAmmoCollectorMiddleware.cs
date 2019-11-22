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

            context.Request.EnableBuffering();

            try
            {
                await next.Invoke(context);
                await StoreRequest(context.Request, context.Response.StatusCode, opts);
            }
            catch (Exception)
            {
                await StoreRequest(context.Request, -1, opts);
                throw;
            }
        }

        private async Task StoreRequest(HttpRequest request, int responseStatusCode, PhantomAmmoCollectorOptions opts)
        {
            try
            {
                var ammo = await MakeAmmo(request, responseStatusCode);
                await StoreAmmo(ammo, opts);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, e.Message);
            }
        }

        private async Task StoreAmmo(PhantomAmmoInfo ammoInfo, PhantomAmmoCollectorOptions opts)
        {
            if (!string.IsNullOrWhiteSpace(opts.AllRequestsFile))
            {
                using (var file = File.AppendText(opts.AllRequestsFile))
                {
                    await file.WriteAsync(ammoInfo.ToString());
                }    
            }
            
            if (!string.IsNullOrWhiteSpace(opts.GoodRequestsFile)
                && ammoInfo.Status == PhantomAmmoStatuses.Good)
            {
                using (var file = File.AppendText(opts.GoodRequestsFile))
                {
                    await file.WriteAsync(ammoInfo.ToString());
                }    
            }
            else if (!string.IsNullOrWhiteSpace(opts.BadRequestsFile))
            {
                using (var file = File.AppendText(opts.BadRequestsFile))
                {
                    await file.WriteAsync(ammoInfo.ToString());
                }    
            }
        }

        private async Task<PhantomAmmoInfo> MakeAmmo(HttpRequest request, int responseStatusCode)
        {
            var url = request.GetEncodedUrl();
            var ammoInfo = new PhantomAmmoInfo(url, request.Method)
            {
                Body = await ExtractBody(request),
                Protocol = request.Protocol,
                Status = GetResponseStatus(responseStatusCode)
            };
            foreach (var h in request.Headers)
            {
                ammoInfo.Headers.Add(h.Key, h.Value);
            }
            return ammoInfo;
        }

        private string GetResponseStatus(int statusCode) =>
            statusCode > 199 && statusCode < 300
                ? PhantomAmmoStatuses.Good
                : PhantomAmmoStatuses.Bad;
        
        private async Task<string> ExtractBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(request.Body);

            return await reader.ReadToEndAsync();
        }
    }
}