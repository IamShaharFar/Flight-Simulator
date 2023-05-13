namespace WebApplication1.Models
{
    public interface IAirport
    {
        int Id { get; set; }
        string? Name { get; set; }
        List<Leg>? Legs { get; set; }
        List<Flight> Flights { get; set; }
        List<ILeg> LandingRoad { get; set; }
        List<ILeg> TakeoffRoad { get; set; }
        Leg GetLeg(int id);
    }
    public class Airport : IAirport
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Leg>? Legs { get; set; } = new List<Leg> {
            new Leg { Id = 1,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 2,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 3,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 4,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = true, WaitingList = new List<IFlight>()} ,
            new Leg { Id = 5,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 6,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = true, WaitingList = new List<IFlight>()} ,
            new Leg { Id = 7,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = true, WaitingList = new List < IFlight >()} ,
            new Leg { Id = 8,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
            new Leg { Id = 9,Airplanes = new List<IFlight>(), ForOneAirplaneOnly = false} ,
        };
        public List<Flight>? Flights { get; set; } = new List<Flight>();
        public List<ILeg> TakeoffRoad { get; set; }
        public List<ILeg> LandingRoad { get; set; }

        public Airport()
        {
            LandingRoad = new List<ILeg>
            {
                GetLeg(1),
                GetLeg(2),
                GetLeg(3),
                GetLeg(4),
                GetLeg(5),
                new MultyLeg { Id = 20,ForOneAirplaneOnly = false, Airplanes = new List<IFlight>(), Stations = new List<ILeg>
                {
                GetLeg(6),
                GetLeg(7)
                } }
            };
            TakeoffRoad = new List<ILeg>
            {
                new MultyLeg { Id = 21,ForOneAirplaneOnly = false, Airplanes = new List<IFlight>(), Stations = new List<ILeg>
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
