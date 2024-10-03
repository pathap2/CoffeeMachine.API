using CoffeeMachine.API.Services.MachineService;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoffeeMachineController(IMachineService machineService) : ControllerBase
{
    private readonly IMachineService _machineService = machineService;

    [HttpGet("BrewCoffee")]
    public async Task<ActionResult> GetBrewCoffeeAsync()
    {
        try
        {
            var (message, isOutOfCoffee) = await _machineService.BrewCoffeeAsync(DateTime.UtcNow);

            if (isOutOfCoffee)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service Unavailable");
            }

            return Ok(new
            {
                message,
                prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
            });
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            return StatusCode(StatusCodes.Status418ImATeapot, ex.Message);
        }
    }
}
