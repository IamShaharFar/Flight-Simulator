using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace WebApplication1.Models
{
    public interface IMultyLeg
    {
        List<ILeg>? Stations { get; }
    }
    public interface ILeg
    {
        int Id { get; set; }
        bool ForOneAirplaneOnly { get; set; }
        List<IFlight>? Airplanes { get; set; }
        List<IFlight>? WaitingList { get; set; }
    }
    public class Leg : ILeg
    {
        public int Id { get; set; }
        public bool ForOneAirplaneOnly { get; set; }
        public List<IFlight>? Airplanes { get; set; } = new List<IFlight>();
        public List<IFlight>? WaitingList { get; set; }
    }
    public class MultyLeg : Leg, IMultyLeg
    {
        public List<ILeg>? Stations { get; set; }
    }



}
