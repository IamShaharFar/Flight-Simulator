namespace WebApplication1.Models
{
    public interface IFlight
    {
        int Id { get; set; }
        string? Airplane { get; set; }
        bool IsLanding { get; set; }
        List<ILeg>? Road { get; set; }
        Tuple<Leg, Leg> CurrentLeg { get; set; }
        DateTime TakeOff { get; set; }
    }
    public class Flight : IFlight
    {
        public int Id { get; set; }
        public string? Airplane { get; set; }
        public bool IsLanding { get; set; }
        public List<ILeg>? Road { get; set; }
        public Tuple<Leg, Leg> CurrentLeg { get; set; }
        public DateTime TakeOff { get; set; }
        
    }
}
