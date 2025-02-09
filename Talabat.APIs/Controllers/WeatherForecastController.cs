using Microsoft.AspNetCore.Mvc;

namespace Talabat.APIs.Controllers
{
    [ApiController]                 // because AddControllers() adds support for API conventions.
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

        [HttpGet(Name = "GetWeatherForecast")]          //This is part of the configuration added by AddControllers().
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //Model binding and validation come into play when you're dealing with incoming data,
        //for example, the Post method uses [FromBody] to bind incoming JSON data to the value parameter.
        [HttpPost(Name = "GetS")]
        public string Get([FromBody] string s)
        {
            return s.ToUpper();
        }
    }
}
