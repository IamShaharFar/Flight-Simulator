using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace WebApplication1.Models
{
    public interface IMultyLeg
    {
        Collection<Leg>? Stations { get; }
    }
    public interface ILeg
    {
        [Key]
        int Id { get; set; }
        bool ForOneAirplaneOnly { get; set; }
        Collection<Flight>? Airplanes { get; set; }
        Collection<Flight>? WaitingList { get; set; }
    }
    public class Leg : ILeg
    {
        [Key]
        public int Id { get; set; }
        public bool ForOneAirplaneOnly { get; set; }
        public Collection<Flight>? Airplanes { get; set; } = new Collection<Flight>();
        public Collection<Flight>? WaitingList { get; set; }
    }
    public class MultyLeg : Leg, IMultyLeg
    {
        public Collection<Leg>? Stations { get; set; }
    }



}
