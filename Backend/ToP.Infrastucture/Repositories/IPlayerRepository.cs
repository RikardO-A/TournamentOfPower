using ToP.Domain.Classes;

namespace ToP.Infrastructure.Repositories
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(int id);
        Task<Player?> GetByIndexAsync(int index);
        Task<Player> AddAsync(Player player);
        Task<bool> RemoveAsync(int id);
        Task<int> GetCountAsync();
        Task<int> SaveChangesAsync();
    }
}
