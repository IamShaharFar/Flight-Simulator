using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public interface IAirport
    {
        [Key]
        int Id { get; set; }
        string? Name { get; set; }
        List<Leg>? Legs { get; set; }
        List<Flight> Flights { get; set; }
        List<Leg> LandingRoad { get; set; }
        List<Leg> TakeoffRoad { get; set; }
        Leg GetLeg(int id);
    }
    public class Airport : IAirport
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Leg>? Legs { get; set; } = new List<Leg> {
            new Leg { Id = 1,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 2,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 3,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 4,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, WaitingList = new List<Flight>()} ,
            new Leg { Id = 5,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 6,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, WaitingList = new List<Flight>()} ,
            new Leg { Id = 7,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, WaitingList = new List <Flight>()} ,
            new Leg { Id = 8,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 9,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false} ,
        };
        public List<Flight>? Flights { get; set; } = new List<Flight>();
        public List<Leg> TakeoffRoad { get; set; }
        public List<Leg> LandingRoad { get; set; }

        public Airport()
        {
            LandingRoad = new List<Leg>
            {
                GetLeg(1),
                GetLeg(2),
                GetLeg(3),
                GetLeg(4),
                GetLeg(5),
                new MultyLeg { Id = 20,ForOneAirplaneOnly = false, Airplanes = new List<Flight>(), Stations = new List<Leg>
                {
                GetLeg(6),
                GetLeg(7)
                } }
            };
            TakeoffRoad = new List<Leg>
            {
                new MultyLeg { Id = 21,ForOneAirplaneOnly = false, Airplanes = new List<Flight>(), Stations = new List<Leg>
                {
                GetLeg(6),
                GetLeg(7)
                } },
                GetLeg(8),
                GetLeg(4),
                GetLeg(9)
            };
        }

        public Leg GetLeg(int id)
        {
            return Legs.FirstOrDefault(Leg => Leg.Id == id);
        }
    }
}
