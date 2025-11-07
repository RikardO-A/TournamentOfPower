
namespace ToP.Domain.Classes
{
    public class Participants
    {
        private readonly List<Player> _players = new();

        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

        public void Add(Player player)
        {
            if (_players.All(p => p.Id != player.Id))
                _players.Add(player);
        }

        public void Remove(int playerId)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
                _players.Remove(player);
        }

        public int Count => _players.Count;
    }
}