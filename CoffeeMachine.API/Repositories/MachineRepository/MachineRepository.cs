using CoffeeMachine.API.Data;
using CoffeeMachine.API.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.API.Repositories.MachineRepository;

public class MachineRepository(AppDbContext context) : IMachineRepository
{
    private readonly AppDbContext _context = context;

    public async Task<CoffeeRequest> GetCoffeeRequestAsync()
    {
        return (await _context.CoffeeRequests.FirstOrDefaultAsync())!;
    }

    public async Task UpdateCoffeeRequestAsync(CoffeeRequest request)
    {
        _context.CoffeeRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}
