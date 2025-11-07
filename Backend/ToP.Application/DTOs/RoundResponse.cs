namespace ToP.Application.DTOs
{
    public class RoundResponse
    {
        public int Round { get; set; }
        public List<PairResponse> Pairs { get; set; } = new();
    }

    public class PairResponse
    {
        public string Home { get; set; } = string.Empty;
        public string Away { get; set; } = string.Empty;
    }
}
