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

        [HttpPost("start")]
        public async Task<IActionResult> StartSimulator()
        {
            _ = Task.Run(() =>
            {
                airportService.StartSim();
            });

            return Ok();
        }

        [HttpPost("addflight")]
        public IActionResult AddFlight() 
        {
            var flight = airportService.AddFlight();
            Task.Run(() => airportService.MakeFlight(flight));
            return Ok(flight); 
        }



    }
}
