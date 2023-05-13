using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace WebApplication1.Models
{
    public enum LegStatus
    {
        Both,
        TakeOff,
        Landing
    }
    public interface ILeg
    {
        int Id { get; set; }
        LegStatus Status { get; set; }
        bool ForOneAirplaneOnly { get; set; }
        List<Flight> Airplanes { get; set; }
    }
    public class Leg : ILeg
    {
        public int Id { get; set; }
        public LegStatus Status { get; set; }
        public bool ForOneAirplaneOnly { get; set; }
        public List<Flight>? Airplanes { get; set; }
    }



}
