using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
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
            await next.Invoke(context);

            var opts = optsAccessor.CurrentValue;
            if (opts.Enabled)
            {
                try
                {
                    var ammo = await MakeAmmo(context);
                    await StoreAmmo(ammo, opts.PathToFile);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, e.Message);
                }
            }
        }

        private async Task<PhantomAmmoInfo> MakeAmmo(HttpContext context)
        {
            var request = context.Request;
            request.EnableBuffering();
            
            var url = request.GetEncodedUrl();
            var ammoInfo = new PhantomAmmoInfo(url, request.Method)
            {
                Body = await ExtractBody(request),
                Protocol = request.Protocol,
                Status = GetResponseStatus(context.Response.StatusCode)
            };
            foreach (var h in request.Headers)
            {
                ammoInfo.Headers.Add(h.Key, h.Value);
            }

            return ammoInfo;
        }

        private string GetResponseStatus(int statusCode) =>
            statusCode > 199 && statusCode < 300
                ? "good"
                : "bad";
        

        private async Task StoreAmmo(PhantomAmmoInfo ammoInfo, string pathToFile)
        {
            using (var file = File.AppendText(pathToFile))
            {
                await file.WriteAsync(ammoInfo.ToString());
            }
        }

        private async Task<string> ExtractBody(HttpRequest request)
        {
            var reader = new StreamReader(request.Body);

            var body = await reader.ReadToEndAsync();

            request.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }
    }
}