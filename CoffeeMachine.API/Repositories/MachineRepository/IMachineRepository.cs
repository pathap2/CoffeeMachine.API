using CoffeeMachine.API.Models.Request;

namespace CoffeeMachine.API.Repositories.MachineRepository;

public interface IMachineRepository
{
    Task<CoffeeRequest> GetCoffeeRequestAsync();
    Task UpdateCoffeeRequestAsync(CoffeeRequest request);
}
