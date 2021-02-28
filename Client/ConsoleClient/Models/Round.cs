
namespace ConsoleClient.Models
{
    public class Round
    {
        public (string, Figure) Winner { get; set; }
        public (string, Figure) Looser { get; set; }

        public Round()
        {
                
        }
    }
    public enum Figure
    {
        None,
        Paper,
        Rock,
        Scissors
    }
}
