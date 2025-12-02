namespace ToP.Domain.Classes
{
    public class GameMatch
    {
        public int Id { get; set; }
        public int Round { get; set; }
        public Player Player1 { get; set; } = null!;
        public Player Player2 { get; set; } = null!;
        public int Player1Wins { get; set; } = 0;
        public int Player2Wins { get; set; } = 0;
        public bool IsComplete { get; set; } = false;
        public int? WinnerId { get; set; }
        public List<MatchRound> Rounds { get; set; } = new();
    }

    public class MatchRound
    {
        public int RoundNumber { get; set; }
        public string? Player1Move { get; set; }
        public string? Player2Move { get; set; }
        public string? Result { get; set; }
    }
}
