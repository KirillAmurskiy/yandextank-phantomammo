using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public ActionResult<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            return Ok(result);
        }
        
        [HttpPost]
        public ActionResult<PostArgs> Post(PostArgs args)
        {
            if (args.Arg2 == "exception")
            {
                throw new Exception("Hello exception!");
            }
            else if (args.Arg2 == "exception1")
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return args;
        }
        
        [HttpPut]
        public ActionResult<PutArgs> Put(PutArgs args)
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