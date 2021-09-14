using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netCoreApi.Entidades.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Controllers
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
        private readonly Irepositorio repositorio;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            Irepositorio repositorio)
        {
            _logger = logger;
            this.repositorio = repositorio;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var generos = repositorio.ObtenerTodosLosGeneros();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("guid")]
        public Guid ObtenerGuidWeatherForecastController()
        {
            return repositorio.ObtenerGuid();
        }
    }
}
