using CoffeeMachine.API.Models.Common;
using CoffeeMachine.API.Models.Request;
using CoffeeMachine.API.Repositories.MachineRepository;
using CoffeeMachine.API.Services.MachineService;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CoffeeMachine.Tests;

public class MachineServiceTests
{
    private readonly Mock<IMachineRepository> _mockRepository;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IOptions<WeatherSettings>> _mockOptions;
    private MachineService _machineService;

    public MachineServiceTests()
    {
        _mockRepository = new Mock<IMachineRepository>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockOptions = new Mock<IOptions<WeatherSettings>>();

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler()));
    }

    [Theory]
    [InlineData(35, "Your refreshing iced coffee is ready")]
    [InlineData(20, "Your piping hot coffee is ready")]
    public async Task BrewCoffeeAsync_ReturnsCorrectMessage(double temperature, string expectedMessage)
    {
        _mockRepository.Setup(repo => repo.GetCoffeeRequestAsync()).ReturnsAsync(new CoffeeRequest
        {
            RequestCount = 1,
            LastRequestDate = DateTime.UtcNow
        });

        _mockOptions.Setup(o => o.Value).Returns(new WeatherSettings
        {
            WeatherAPIUrl = "https://api.openweathermap.org/data/2.5/weather?q=Auckland&appid=testappid&units=metric"
        });

        var fakeHttpClient = new HttpClient(new FakeHttpMessageHandler(temperature));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        _machineService = new MachineService(_mockRepository.Object, _mockHttpClientFactory.Object, _mockOptions.Object);

        var result = await _machineService.BrewCoffeeAsync(DateTime.UtcNow);

        Assert.Equal(expectedMessage, result.message);
    }

    [Fact]
    public async Task BrewCoffeeAsync_OnFirstApril_ThrowsHttpRequestExceptionWith503()
    {
        _mockRepository.Setup(repo => repo.GetCoffeeRequestAsync()).ReturnsAsync(new CoffeeRequest
        {
            RequestCount = 1,
            LastRequestDate = DateTime.UtcNow
        });

        _machineService = new MachineService(_mockRepository.Object, _mockHttpClientFactory.Object, _mockOptions.Object);

        var date = new DateTime(2024, 4, 1);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _machineService.BrewCoffeeAsync(date));

        Assert.Equal("I'm a teapot", exception.Message);
        Assert.Equal(System.Net.HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }

    [Fact]
    public async Task BrewCoffeeAsync_RequestCountMultipleOfFive_ReturnsNullAndTrue()
    {
        _mockRepository.Setup(repo => repo.GetCoffeeRequestAsync()).ReturnsAsync(new CoffeeRequest
        {
            RequestCount = 4,
            LastRequestDate = DateTime.UtcNow
        });

        _machineService = new MachineService(_mockRepository.Object, _mockHttpClientFactory.Object, _mockOptions.Object);

        var result = await _machineService.BrewCoffeeAsync(DateTime.UtcNow);

        _mockRepository.Verify(repo => repo.UpdateCoffeeRequestAsync(It.IsAny<CoffeeRequest>()), Times.Once);
        Assert.Null(result.message);
        Assert.Equal(result.isOutOfCoffee, true);
    }
}


public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly double _temperature;

    public FakeHttpMessageHandler(double temperature = 30) => _temperature = temperature;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"main\": {{\"temp\": {_temperature}}}}}")
        };

        return Task.FromResult(response);
    }
}
