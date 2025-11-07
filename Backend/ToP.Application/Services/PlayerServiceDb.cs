using ToP.Domain.Classes;
using ToP.Application.Interfaces;
using ToP.Infrastructure.Repositories;

namespace ToP.Application.Services
{
    public class PlayerServiceDb : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly IRoundRobinService? _roundRobinService;

        public PlayerServiceDb(IPlayerRepository repository, IRoundRobinService? roundRobinService = null)
        {
            _repository = repository;
            _roundRobinService = roundRobinService;
        }

        // Async methods (recommended)
        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Player?> GetPlayerByIndexAsync(int index)
        {
            return await _repository.GetByIndexAsync(index);
        }

        public async Task<Player> AddPlayerAsync(string name, string? image = null)
        {
            // Get max ID to generate next ID
            var allPlayers = await _repository.GetAllAsync();
            int nextId = allPlayers.Any() ? allPlayers.Max(p => p.Id) + 1 : 1;

            var player = new Player
            {
                Id = nextId,
                Name = name,
                Image = image ?? string.Empty
            };

            await _repository.AddAsync(player);
            _roundRobinService?.InvalidateCache();

            return player;
        }

        public async Task<bool> RemovePlayerAsync(int id)
        {
            var result = await _repository.RemoveAsync(id);
            
            if (result)
            {
                _roundRobinService?.InvalidateCache();
            }
            
            return result;
        }

        public async Task<int> GetPlayerCountAsync()
        {
            return await _repository.GetCountAsync();
        }

        // Synchronous methods for backward compatibility (delegates to async)
        public List<Player> GetAllPlayers()
        {
            return GetAllPlayersAsync().GetAwaiter().GetResult();
        }

        public Player? GetPlayerById(int id)
        {
            return GetPlayerByIdAsync(id).GetAwaiter().GetResult();
        }

        public Player? GetPlayerByIndex(int index)
        {
            return GetPlayerByIndexAsync(index).GetAwaiter().GetResult();
        }

        public Player AddPlayer(string name, string? image = null)
        {
            return AddPlayerAsync(name, image).GetAwaiter().GetResult();
        }

        public bool RemovePlayer(int id)
        {
            return RemovePlayerAsync(id).GetAwaiter().GetResult();
        }

        public int GetPlayerCount()
        {
            return GetPlayerCountAsync().GetAwaiter().GetResult();
        }
    }
}
