namespace WebApplication1.Models
{
    public interface IAirport
    {
        int Id { get; set; }
        string? Name { get; set; }
        List<Leg>? Legs { get; set; }
        List<Flight> Flights { get; set; }
        Leg GetLeg(int id);
    }
    public class Airport : IAirport
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Leg>? Legs { get; set; } = new List<Leg> {
            new Leg { Id = 1,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
            new Leg { Id = 2,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
            new Leg { Id = 3,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
            new Leg { Id = 4,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, Status = LegStatus.Both} ,
            new Leg { Id = 5,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
            new Leg { Id = 6,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, Status = LegStatus.Both} ,
            new Leg { Id = 7,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, Status = LegStatus.Both} ,
            new Leg { Id = 8,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.TakeOff} ,
            new Leg { Id = 9,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.TakeOff} ,
        };
        public List<Leg> LandingRoad = new List<Leg> { new Leg { Id = 1,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
                                                       new Leg { Id = 2,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
                                                       new Leg { Id = 3,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} ,
                                                       new Leg { Id = 4,Airplanes = new List<Flight>(), ForOneAirplaneOnly = true, Status = LegStatus.Both} ,
                                                       new Leg { Id = 5,Airplanes = new List<Flight>(), ForOneAirplaneOnly = false, Status = LegStatus.Landing} };
        public List<Flight>? Flights { get; set; }

        public Leg GetLeg(int id)
        {
            return Legs.FirstOrDefault(Leg => Leg.Id == id);
        }
    }
}
