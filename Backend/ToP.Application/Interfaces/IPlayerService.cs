using ToP.Domain.Classes;

namespace ToP.Application.Interfaces
{
    public interface IPlayerService
    {
        List<Player> GetAllPlayers();
        Player? GetPlayerById(int id);
        Player? GetPlayerByIndex(int index);
        Player AddPlayer(string name, string? image = null);
        bool RemovePlayer(int id);
        int GetPlayerCount();
    }
}
