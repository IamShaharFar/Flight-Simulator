using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class FlightHub : Hub
    {
        public async Task SendNextLegAndFlightId(int nextLegId, int flightId)
        {
            await Clients.All.SendAsync("MovedFlight", nextLegId, flightId);
        }
    }
}
