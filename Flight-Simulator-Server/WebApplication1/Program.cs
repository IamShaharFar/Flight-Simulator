using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using WebApplication1.Controllers;
using WebApplication1.Dal;
using WebApplication1.Hubs;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAirport, Airport>();
builder.Services.AddSingleton<IAirportService, AirportService>();
builder.Services.AddDbContext<FlightsDB>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Flights")), ServiceLifetime.Singleton);
builder.Services.AddSingleton<IMongoClient>(s =>
            new MongoClient(builder.Configuration.GetConnectionString("FlightSimulator")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.MapHub<FlightHub>("/flighthub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
