using System.Collections.Generic;
using ToP.Domain.Classes;

namespace ToP.Application.Interfaces
{
    public interface IRoundRobinService
    {
        List<Match> GenerateFixtures(List<string> playerNames);
        List<Match> GetMatchesForRound(List<Player> players, int round);
        int GetMaxRounds(int numberOfPlayers);
        int GetRemainingMatches(int numberOfPlayers, int roundsPlayed);
        Player? GetOpponentForPlayerInRound(List<Player> players, int playerIndex, int round);
        List<(int Round, Player Opponent)> GetPlayerSchedule(List<Player> players, int playerIndex);
        void InvalidateCache();
    }
}
