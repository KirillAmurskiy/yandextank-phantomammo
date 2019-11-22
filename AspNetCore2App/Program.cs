using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore2App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build().Run();
        }

    }
}