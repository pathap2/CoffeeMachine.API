namespace CoffeeMachine.API.Services.MachineService;

public interface IMachineService
{
    Task<(string message, bool isOutOfCoffee)> BrewCoffeeAsync(DateTime today);
}
