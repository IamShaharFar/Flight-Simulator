using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class LegStation
    {
        [Key]
        public int Id { get; set; }
        public Leg MultyLeg { get; set; }
        public Leg SubLeg { get; set; }
        public LegStation()
        {
       
        }
    }
}
