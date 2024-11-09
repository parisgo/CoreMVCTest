using CoreMVCTest.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoreMVCTest.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IUserService _userService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IUserService userService, ILogger<WeatherForecastController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpGet(Name = "GetWeatherForecast")]
        //[CacheDataFilter(TTL = 600, IsUseToken = false)]        
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("web api get *************************");

            //_userService.GetList();
            _userService.GetById(1);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
