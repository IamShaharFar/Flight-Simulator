using Microsoft.VisualBasic;
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
        Collection<Leg>? Road { get; set; }
        LegStation CurrentLeg { get; set; }
        DateTime TakeOff { get; set; }
    }
    public class Flight : IFlight
    {
        [Key]
        public int Id { get; set; }
        public string? Airplane { get; set; }
        public bool IsLanding { get; set; }
        [NotMapped]
        public Collection<Leg>? Road { get; set; }
        public LegStation CurrentLeg { get; set; }
        public DateTime TakeOff { get; set; }
        
    }
}
