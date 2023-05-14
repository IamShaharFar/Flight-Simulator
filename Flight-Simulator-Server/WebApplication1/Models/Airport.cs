using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public interface IAirport
    {
        [Key]
        int Id { get; set; }
        string? Name { get; set; }
        Collection<Leg>? Legs { get; set; }
        Collection<Flight> Flights { get; set; }
        Collection<Leg> LandingRoad { get; set; }
        Collection<Leg> TakeoffRoad { get; set; }
        Leg GetLeg(int id);
    }
    public class Airport : IAirport
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public Collection<Leg>? Legs { get; set; } = new Collection<Leg> {
            new Leg { Id = 1,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 2,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 3,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 4,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = true, WaitingList = new Collection<Flight>()} ,
            new Leg { Id = 5,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 6,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = true, WaitingList = new Collection<Flight>()} ,
            new Leg { Id = 7,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = true, WaitingList = new Collection <Flight>()} ,
            new Leg { Id = 8,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 9,Airplanes = new Collection<Flight>(), ForOneAirplaneOnly = false} ,
        };
        public Collection<Flight>? Flights { get; set; } = new Collection<Flight>();
        public Collection<Leg> TakeoffRoad { get; set; }
        public Collection<Leg> LandingRoad { get; set; }

        public Airport()
        {
            LandingRoad = new Collection<Leg>
            {
                GetLeg(1),
                GetLeg(2),
                GetLeg(3),
                GetLeg(4),
                GetLeg(5),
                new MultyLeg { Id = 20,ForOneAirplaneOnly = false, Airplanes = new Collection<Flight>(), Stations = new Collection<Leg>
                {
                GetLeg(6),
                GetLeg(7)
                } }
            };
            TakeoffRoad = new Collection<Leg>
            {
                new MultyLeg { Id = 21,ForOneAirplaneOnly = false, Airplanes = new Collection<Flight>(), Stations = new Collection<Leg>
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
