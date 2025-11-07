using ToP.Domain.Classes;
using ToP.Application.Interfaces;

namespace ToP.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly List<Player> _players;
        private int _nextId = 21;
        private readonly IRoundRobinService? _roundRobinService;

        public PlayerService(IRoundRobinService? roundRobinService = null)
        {
            _roundRobinService = roundRobinService;
            
            // Initialize with sample data from CiP-02
            _players = new List<Player>
            {
                new Player { Id = 1, Name = "Alice" },
                new Player { Id = 2, Name = "Bob" },
                new Player { Id = 3, Name = "Charlie" },
                new Player { Id = 4, Name = "Diana" },
                new Player { Id = 5, Name = "Ethan" },
                new Player { Id = 6, Name = "Fiona" },
                new Player { Id = 7, Name = "George" },
                new Player { Id = 8, Name = "Hannah" },
                new Player { Id = 9, Name = "Isaac" },
                new Player { Id = 10, Name = "Julia" },
                new Player { Id = 11, Name = "Kevin" },
                new Player { Id = 12, Name = "Laura" },
                new Player { Id = 13, Name = "Michael" },
                new Player { Id = 14, Name = "Nina" },
                new Player { Id = 15, Name = "Oscar" },
                new Player { Id = 16, Name = "Paula" },
                new Player { Id = 17, Name = "Quentin" },
                new Player { Id = 18, Name = "Rachel" },
                new Player { Id = 19, Name = "Samuel" },
                new Player { Id = 20, Name = "Tina" }
            };
        }

        public List<Player> GetAllPlayers()
        {
            return _players.ToList();
        }

        public Player? GetPlayerById(int id)
        {
            return _players.FirstOrDefault(p => p.Id == id);
        }

        public Player? GetPlayerByIndex(int index)
        {
            if (index < 0 || index >= _players.Count)
                return null;
            return _players[index];
        }

        public Player AddPlayer(string name, string? image = null)
        {
            var player = new Player
            {
                Id = _nextId++,
                Name = name,
                Image = image ?? string.Empty
            };
            _players.Add(player);
            
            // Invalidate round-robin cache when player list changes
            _roundRobinService?.InvalidateCache();
            
            return player;
        }

        public bool RemovePlayer(int id)
        {
            var player = GetPlayerById(id);
            if (player == null)
                return false;
            
            bool removed = _players.Remove(player);
            
            // Invalidate round-robin cache when player list changes
            if (removed)
            {
                _roundRobinService?.InvalidateCache();
            }
            
            return removed;
        }

        public int GetPlayerCount()
        {
            return _players.Count;
        }
    }
}
