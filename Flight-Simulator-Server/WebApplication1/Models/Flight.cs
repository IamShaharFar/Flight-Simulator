using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public interface IFlight
    {
        [Key]
        int Id { get; set; }
        string? Airplane { get; set; }
        bool IsLanding { get; set; }
        [NotMapped]
        List<Leg>? Road { get; set; }
        LegStation CurrentLeg { get; set; }
        DateTime TakeOff { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Flight : IFlight
    {
        [Key]
        public int Id { get; set; }
        public string? Airplane { get; set; }
        public bool IsLanding { get; set; }
        [BsonIgnore]
        public List<Leg>? Road { get; set; }
        [BsonIgnore]
        public LegStation CurrentLeg { get; set; }
        public DateTime TakeOff { get; set; }

        [BsonElement("CurrentLeg.MultyLeg.Id")]
        public int? CurrentLegMultyLegId
        {
            get { return CurrentLeg?.MultyLeg?.Id; }
            set
            {
                if (CurrentLeg == null)
                    CurrentLeg = new LegStation();

                if (CurrentLeg.MultyLeg == null)
                    CurrentLeg.MultyLeg = new MultyLeg();

                CurrentLeg.MultyLeg.Id = (int)(value == null ? -1 : value);
            }
        }

        [BsonElement("CurrentLeg.SubLeg.Id")]
        public int? CurrentLegSubLegId
        {
            get { return CurrentLeg?.SubLeg?.Id; }
            set
            {
                if (CurrentLeg == null)
                    CurrentLeg = new LegStation();

                if (CurrentLeg.SubLeg == null)
                    CurrentLeg.SubLeg = new Leg();

                CurrentLeg.SubLeg.Id = (int)(value == null ? -1 : value);
            }
        }
    }
}
