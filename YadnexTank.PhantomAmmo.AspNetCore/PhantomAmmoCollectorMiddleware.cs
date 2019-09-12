using System;
using System.IO;
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
            var opts = optsAccessor.CurrentValue;
            if (opts.Enabled)
            {
                try
                {
                    await CollectRequest(context.Request, opts.PathToFile);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, e.Message);
                }
            }
		    
            await next.Invoke(context);
        }

        private async Task CollectRequest(HttpRequest request, string pathToFile)
        {
            using (var file = File.AppendText(pathToFile))
            {
                var url = request.GetEncodedUrl();

                request.EnableRewind();

                var ammoInfo = new PhantomAmmoInfo(url, request.Method)
                {
                    Body = await ExtractBody(request),
                    Protocol = request.Protocol
                };
                foreach (var h in request.Headers)
                {
                    ammoInfo.Headers.Add(h.Key, h.Value);
                }

                file.Write(ammoInfo.ToString());
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