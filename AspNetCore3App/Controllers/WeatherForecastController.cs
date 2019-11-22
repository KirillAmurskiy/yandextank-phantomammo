using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore3App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
        
        [HttpPost]
        public PostArgs Post(PostArgs args)
        {
            if (args.Arg2 == "exception")
            {
                throw new Exception("Hello exception!");
            }
            return args;
        }
        
        [HttpPut]
        public PutArgs Put(PutArgs args)
        {
            return args;
        }
    }

    public class PostArgs
    {
        public int Arg1 { get; set; }
        
        public string Arg2 { get; set; }
    }
    
    public class PutArgs
    {
        public int Arg1 { get; set; }
        
        public string Arg2 { get; set; }
    }
}