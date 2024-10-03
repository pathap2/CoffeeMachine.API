using CoffeeMachine.API.Models.Common;
using CoffeeMachine.API.Models.Request;
using CoffeeMachine.API.Repositories.MachineRepository;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CoffeeMachine.API.Services.MachineService;

public class MachineService(IMachineRepository machineRepository, IHttpClientFactory httpClient, IOptions<WeatherSettings> options) : IMachineService
{
    private readonly IMachineRepository _machineRepository = machineRepository;
    private readonly IHttpClientFactory _httpClient = httpClient;
    private string _weatherApiUrl = options?.Value?.WeatherAPIUrl!;
    private int _weatherThreshold = (int)options?.Value?.WeatherThreshold!;

    public async Task<(string message, bool isOutOfCoffee)> BrewCoffeeAsync(DateTime today)
    {
        
        if (today.Month == 4 && today.Day == 1)
        {
            throw new HttpRequestException("I'm a teapot", null, System.Net.HttpStatusCode.ServiceUnavailable);
        }

        var coffeeRequest = await _machineRepository.GetCoffeeRequestAsync();
        if (coffeeRequest == null)
        {
            coffeeRequest = new CoffeeRequest { RequestCount = 0, LastRequestDate = today };
        }

        coffeeRequest.RequestCount++;

        if (coffeeRequest.RequestCount % 5 == 0)
        {
            await _machineRepository.UpdateCoffeeRequestAsync(coffeeRequest);
            return (null, true)!;
        }

        var temperature = await GetCurrentTemperatureAsync();
        var message = temperature > _weatherThreshold ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready";

        coffeeRequest.LastRequestDate = today;
        await _machineRepository.UpdateCoffeeRequestAsync(coffeeRequest);

        return (message, false);
    }

    private async Task<double> GetCurrentTemperatureAsync()
    {
        try
        {
            if(!string.IsNullOrWhiteSpace(_weatherApiUrl) && _weatherApiUrl.Contains("{city}"))
            {
                _weatherApiUrl = _weatherApiUrl?.Replace("{city}", "Auckland")!;
            }

            var client = _httpClient.CreateClient();
            var response = await client.GetStringAsync(_weatherApiUrl);
            if (response is not null)
            {
                var jsonObject = JsonDocument.Parse(response);
                if (jsonObject is not null)
                {
                    return jsonObject.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return default;
    }
}
