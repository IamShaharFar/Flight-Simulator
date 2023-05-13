using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IAirport? airport;
        private readonly IAirportService? airportService;

        public AirportController(IAirport airport, IAirportService airportService)
        {
            this.airport = airport;
            this.airportService = airportService;
        }

        [HttpGet("start")]
        public async Task<IActionResult> StartSimulator()
        {
            var list = await airportService.StartSim();
            var flights = airport.Flights;
            foreach (var flight in flights) 
            {
                Console.WriteLine($"flight - {flight.Id}");
                foreach(var leg in flight.Road)
                {
                    Console.WriteLine($"    leg - {leg.Id}");
                }
            }
            return Ok();
        }
    }
}
