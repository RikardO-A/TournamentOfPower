namespace ToP.Application.DTOs
{
    public class PlayerScheduleResponse
    {
        public string Player { get; set; } = string.Empty;
        public int N { get; set; }
        public List<ScheduleEntry> Schedule { get; set; } = new();
    }

    public class ScheduleEntry
    {
        public int Round { get; set; }
        public string Opponent { get; set; } = string.Empty;
    }
}
