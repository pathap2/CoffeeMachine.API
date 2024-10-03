using CoffeeMachine.API.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.API.Data;

//public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
//{
//    public DbSet<CoffeeRequest> CoffeeRequests { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//    }
//}
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<CoffeeRequest> CoffeeRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CoffeeRequest>(entity =>
        {
            entity.ToTable("CoffeeRequests");
            entity.HasKey(s => s.Id);

            // Seed Data
            entity.HasData(
                new CoffeeRequest
                {
                    Id = 1,
                    RequestCount = 0,
                    LastRequestDate = new DateTime(2025, 12, 31)
                }
            );
        });
    }
}
