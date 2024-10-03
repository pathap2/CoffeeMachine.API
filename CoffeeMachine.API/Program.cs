using CoffeeMachine.API.Data;
using CoffeeMachine.API.Models.Common;
using CoffeeMachine.API.Repositories.MachineRepository;
using CoffeeMachine.API.Services.MachineService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<WeatherSettings>(builder.Configuration.GetSection("WeatherSettings"));

builder.Services.AddHttpClient();
// register services and repositorys
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and ensure the database is created automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure the database is created
    dbContext.Database.EnsureCreated();

    // Apply any pending migrations
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
