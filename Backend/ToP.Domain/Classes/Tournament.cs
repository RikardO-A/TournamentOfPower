namespace ToP.Domain.Classes
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentRound { get; set; } = 1;
        public bool IsComplete { get; set; } = false;
        public List<Player> Players { get; set; } = new();
        public Dictionary<int, int> Scores { get; set; } = new();
        public List<GameMatch> Matches { get; set; } = new();
        public int? CurrentPlayerIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
