using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public interface IMultyLeg
    {
        List<Leg>? Stations { get; }
    }
    public interface ILeg
    {
        [Key]
        int Id { get; set; }
        bool ForOneAirplaneOnly { get; set; }
        List<Flight>? Airplanes { get; set; }
        List<Flight>? WaitingList { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Leg : ILeg
    {
        [Key]
        public int Id { get; set; }
        public bool ForOneAirplaneOnly { get; set; }
        public List<Flight>? Airplanes { get; set; } = new List<Flight>();
        public List<Flight>? WaitingList { get; set; }
    }
    public class MultyLeg : Leg, IMultyLeg
    {
        public List<Leg>? Stations { get; set; }
        public List<Flight>? WaitForAll { get; set; } = new List<Flight> ();
    }



}
