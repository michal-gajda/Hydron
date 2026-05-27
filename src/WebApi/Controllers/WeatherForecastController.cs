using Microsoft.AspNetCore.Mvc;

namespace Hydron.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed partial class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Balmy",
        "Bracing",
        "Chilly",
        "Cool",
        "Freezing",
        "Hot",
        "Mild",
        "Scorching",
        "Sweltering",
        "Warm",
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public Task<IEnumerable<WeatherForecast>> GetAsync(CancellationToken cancellationToken = default)
    {
        LogGetAsync(1, 5);

        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        })
        .ToArray();

        LogDescription(LogLevel.Information, "Weather forecast data generated.");

        return Task.FromResult((IEnumerable<WeatherForecast>)result);
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Data range from {from} to {to}.")]
    public partial void LogGetAsync(int from, int to);

    [LoggerMessage(EventId = 2002, Message = "Description: {description}")]
    public partial void LogDescription(LogLevel level, string description);
}
