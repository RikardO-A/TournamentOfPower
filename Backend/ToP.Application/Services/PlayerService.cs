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

            // Initialize with Anime Tournament participants
            _players = new List<Player>
            {
                new Player { Id = 1, Name = "Goku", Image = "/images/players/goku.png" },
                new Player { Id = 2, Name = "Vegeta", Image = "/images/players/vegeta.png" },
                new Player { Id = 3, Name = "Gohan", Image = "/images/players/gohan.png" },
                new Player { Id = 4, Name = "Frieza", Image = "/images/players/frieza.png" },
                new Player { Id = 5, Name = "Trunks", Image = "/images/players/trunks.png" },
                new Player { Id = 6, Name = "Majin Buu", Image = "/images/players/majinbuu.png" },
                new Player { Id = 7, Name = "Aizen", Image = "/images/players/aizen.png" },
                new Player { Id = 8, Name = "Madara", Image = "/images/players/madara.png" },
                new Player { Id = 9, Name = "Orochimaru", Image = "/images/players/orochimaru.png" },
                new Player { Id = 10, Name = "Ulquiorra", Image = "/images/players/ulqiorra.png" },
                new Player { Id = 11, Name = "Kvothe", Image = "/images/players/kvothe.png" },
                new Player { Id = 12, Name = "Belgarion", Image = "/images/players/belgarion.png" }
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
            // Auto-assign image path if not provided
            if (string.IsNullOrEmpty(image))
            {
                image = $"/images/players/{name.ToLower()}.png";
            }

            var player = new Player
            {
                Id = _nextId++,
                Name = name,
                Image = image
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
