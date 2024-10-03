namespace CoffeeMachine.API.Models.Common;

public class WeatherSettings
{
    public string? WeatherAPIUrl { get; set; }
    public int WeatherThreshold { get; set; } = 30;
}
