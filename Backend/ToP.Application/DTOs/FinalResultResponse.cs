namespace ToP.Application.DTOs
{
    public class FinalResultResponse
    {
        public int WinnerId { get; set; }
        public string WinnerName { get; set; } = string.Empty;
        public Dictionary<int,int> FinalScores { get; set; } = new();
        public int TotalRounds { get; set; }
    }
}
