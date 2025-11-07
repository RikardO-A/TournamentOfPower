namespace ToP.Application.DTOs
{
    public class MatchResponse
    {
        public int Round { get; set; }
        public int PlayerIndex { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Opponent { get; set; } = string.Empty;
    }
}
