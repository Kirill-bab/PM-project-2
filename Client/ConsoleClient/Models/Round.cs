
namespace ConsoleClient.Models
{
    public class Round
    {
        public string Winner { get; set; }
        public string Looser { get; set; }
        public Figure WinnerFigure { get; set; }
        public Figure LooserFigure { get; set; }
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
