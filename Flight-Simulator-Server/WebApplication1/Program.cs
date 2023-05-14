using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.Dal;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAirport, Airport>();
builder.Services.AddSingleton<IAirportService, AirportService>();
builder.Services.AddDbContext<FlightsDB>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Flights")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlightsDB>();
    if(!dbContext.Database.EnsureCreated())
    {
        Console.WriteLine("Database already exists.");
    }
    else
    {
        Console.WriteLine("Database created successfully.");
    }
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
