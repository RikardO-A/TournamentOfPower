namespace ToP.Application.DTOs
{
    public class TournamentStartRequest
    {
        public string Name { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
    }

    public class PlayMoveRequest
    {
        public string Move { get; set; } = string.Empty;
    }
}
