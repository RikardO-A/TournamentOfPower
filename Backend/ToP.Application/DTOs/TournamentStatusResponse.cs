namespace ToP.Application.DTOs
{
    public class TournamentStatusResponse
    {
        public int CurrentRound { get; set; }
        public int CurrentPlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string OpponentName { get; set; } = string.Empty;
        public int OpponentId { get; set; }
        public Dictionary<int, int> Scores { get; set; } = new();
        public int PlayerWins { get; set; }
        public int OpponentWins { get; set; }
        public int RoundsPlayed { get; set; }
        public int CurrentSubRound => IsMatchComplete ? RoundsPlayed : RoundsPlayed + 1;
        public int RoundLimit { get; set; } = 3;
        public int MaxRounds { get; set; }
        public bool IsMatchComplete { get; set; }
        public bool IsTournamentComplete { get; set; }
        public bool AllRoundMatchesComplete { get; set; }
        public List<PlayerInfo> Players { get; set; } = new();
    }

    public class PlayerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
