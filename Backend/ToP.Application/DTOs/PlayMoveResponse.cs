namespace ToP.Application.DTOs
{
    public class PlayMoveResponse
    {
        public string PlayerMove { get; set; } = string.Empty;
        public string OpponentMove { get; set; } = string.Empty;
        public string RoundResult { get; set; } = string.Empty; // Player1, Player2, Draw
        public int PlayerWins { get; set; }
        public int OpponentWins { get; set; }
        public int CurrentRoundNumber { get; set; }
        public bool IsMatchComplete { get; set; }
        public int? MatchWinnerId { get; set; }
        public Dictionary<int,int> UpdatedScores { get; set; } = new();
    }
}
