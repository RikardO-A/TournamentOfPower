using Microsoft.EntityFrameworkCore;
using ToP.Domain.Classes;
using ToP.Infrastructure.Data;

namespace ToP.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly TournamentDbContext _context;

        public PlayerRepository(TournamentDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Player?> GetByIndexAsync(int index)
        {
            var players = await GetAllAsync();
            
            if (index < 0 || index >= players.Count)
                return null;
                
            return players[index];
        }

        public async Task<Player> AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var player = await GetByIdAsync(id);
            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Players.CountAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
