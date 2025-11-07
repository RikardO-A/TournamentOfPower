using System.Collections.Generic;
using System.Linq;
using ToP.Domain.Classes;
using ToP.Application.Interfaces;

namespace ToP.Application.Services
{
    public class RoundRobinService : IRoundRobinService
    {
        // Cache for storing generated tournament data
        private TournamentCache? _cache;
        private readonly object _cacheLock = new object();

        private class TournamentCache
        {
            public int PlayerCount { get; set; }
            public List<Player> Players { get; set; } = new();
            public List<List<Match>> RoundMatches { get; set; } = new(); // Index 0 = Round 1
            public Dictionary<int, List<(int Round, Player Opponent)>> PlayerSchedules { get; set; } = new();
            public int MaxRounds { get; set; }
            public int TotalMatches { get; set; }
        }

        private TournamentCache GetOrCreateCache(List<Player> players)
        {
            lock (_cacheLock)
            {
                // Check if cache is valid
                if (_cache != null && _cache.PlayerCount == players.Count)
                {
                    // Verify players are still the same (by checking IDs or names)
                    bool playersMatch = _cache.Players.Count == players.Count;
                    if (playersMatch)
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (_cache.Players[i].Id != players[i].Id || 
                                _cache.Players[i].Name != players[i].Name)
                            {
                                playersMatch = false;
                                break;
                            }
                        }
                    }

                    if (playersMatch)
                    {
                        return _cache;
                    }
                }

                // Generate new cache
                _cache = GenerateCache(players);
                return _cache;
            }
        }

        private TournamentCache GenerateCache(List<Player> players)
        {
            var cache = new TournamentCache
            {
                PlayerCount = players.Count,
                Players = players.Select(p => new Player 
                { 
                    Id = p.Id, 
                    Name = p.Name, 
                    Image = p.Image 
                }).ToList()
            };

            if (players.Count < 2)
            {
                cache.MaxRounds = 0;
                cache.TotalMatches = 0;
                return cache;
            }

            // Prepare for the Circle Method
            var workingPlayers = new List<Player>(players);
            bool hasBye = workingPlayers.Count % 2 != 0;
            if (hasBye)
            {
                workingPlayers.Add(new Player { Name = "BYE" });
            }

            int totalPlayers = workingPlayers.Count;
            int totalRounds = totalPlayers - 1;
            cache.MaxRounds = totalRounds;

            Player fixedPlayer = workingPlayers[0];
            List<Player> rotatingPlayers = workingPlayers.GetRange(1, totalPlayers - 1);

            // Generate all rounds at once
            for (int round = 0; round < totalRounds; round++)
            {
                var roundMatches = new List<Match>();

                // Pair the fixed player with the top rotating player
                Player topRotatingPlayer = rotatingPlayers[0];
                if (fixedPlayer.Name != "BYE" && topRotatingPlayer.Name != "BYE")
                {
                    roundMatches.Add(new Match(fixedPlayer, topRotatingPlayer));
                }

                // Pair the remaining rotating players
                int half = rotatingPlayers.Count / 2;
                for (int i = 1; i < half + 1; i++)
                {
                    Player p1 = rotatingPlayers[i];
                    Player p2 = rotatingPlayers[rotatingPlayers.Count - i];

                    if (p1.Name != "BYE" && p2.Name != "BYE")
                    {
                        roundMatches.Add(new Match(p1, p2));
                    }
                }

                cache.RoundMatches.Add(roundMatches);

                // Rotate (Circle Method)
                Player lastPlayer = rotatingPlayers.Last();
                rotatingPlayers.RemoveAt(rotatingPlayers.Count - 1);
                rotatingPlayers.Insert(0, lastPlayer);
            }

            cache.TotalMatches = cache.RoundMatches.Sum(r => r.Count);

            // Pre-compute player schedules
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                var schedule = new List<(int Round, Player Opponent)>();
                var targetPlayer = players[playerIndex];

                for (int roundIndex = 0; roundIndex < cache.RoundMatches.Count; roundIndex++)
                {
                    var roundMatches = cache.RoundMatches[roundIndex];
                    foreach (var match in roundMatches)
                    {
                        Player? opponent = null;
                        
                        if (match.Player1.Id == targetPlayer.Id || match.Player1.Name == targetPlayer.Name)
                            opponent = match.Player2;
                        else if (match.Player2.Id == targetPlayer.Id || match.Player2.Name == targetPlayer.Name)
                            opponent = match.Player1;

                        if (opponent != null)
                        {
                            schedule.Add((roundIndex + 1, opponent)); // Round is 1-based
                            break;
                        }
                    }
                }

                cache.PlayerSchedules[playerIndex] = schedule;
            }

            return cache;
        }

        public List<Match> GenerateFixtures(List<string> playerNames)
        {
            var players = playerNames.Select(name => new Player { Name = name }).ToList();
            var matches = new List<Match>();

            if (players.Count < 2) return matches;

            // 1. Prepare for the Circle Method
            bool hasBye = players.Count % 2 != 0;
            if (hasBye)
            {
                // Add a temporary 'BYE' player
                players.Add(new Player { Name = "BYE" });
            }

            int totalPlayers = players.Count;
            int totalRounds = totalPlayers - 1;

            Player fixedPlayer = players[0];
            List<Player> rotatingPlayers = players.GetRange(1, totalPlayers - 1);

            for (int round = 0; round < totalRounds; round++)
            {
                // 2. Pair the fixed player with the top rotating player
                Player topRotatingPlayer = rotatingPlayers[0];

                if (fixedPlayer.Name != "BYE" && topRotatingPlayer.Name != "BYE")
                {
                    matches.Add(new Match(fixedPlayer, topRotatingPlayer));
                }

                // 3. Pair the remaining rotating players
                int half = rotatingPlayers.Count / 2;
                for (int i = 1; i < half + 1; i++)
                {
                    Player p1 = rotatingPlayers[i];
                    Player p2 = rotatingPlayers[rotatingPlayers.Count - i];

                    if (p1.Name != "BYE" && p2.Name != "BYE")
                    {
                        matches.Add(new Match(p1, p2));
                    }
                }

                // 4. Rotate (Circle Method)
                Player lastPlayer = rotatingPlayers.Last();
                rotatingPlayers.RemoveAt(rotatingPlayers.Count - 1);
                rotatingPlayers.Insert(0, lastPlayer);
            }

            return matches;
        }

        public List<Match> GetMatchesForRound(List<Player> players, int round)
        {
            if (players.Count < 2) return new List<Match>();
            
            var cache = GetOrCreateCache(players);
            
            if (round < 1 || round > cache.MaxRounds)
                return new List<Match>();

            return cache.RoundMatches[round - 1]; // Round is 1-based, list is 0-based
        }

        public int GetMaxRounds(int numberOfPlayers)
        {
            if (numberOfPlayers < 2) return 0;
            return numberOfPlayers % 2 == 0 ? numberOfPlayers - 1 : numberOfPlayers;
        }

        public int GetRemainingMatches(int numberOfPlayers, int roundsPlayed)
        {
            if (numberOfPlayers < 2) return 0;
            
            int n = numberOfPlayers % 2 == 0 ? numberOfPlayers : numberOfPlayers + 1;
            int totalMatches = n * (n - 1) / 2;
            int matchesPerRound = n / 2;
            int playedMatches = roundsPlayed * matchesPerRound;
            
            return totalMatches - playedMatches;
        }

        public Player? GetOpponentForPlayerInRound(List<Player> players, int playerIndex, int round)
        {
            if (playerIndex < 0 || playerIndex >= players.Count) return null;
            
            var cache = GetOrCreateCache(players);
            
            if (round < 1 || round > cache.MaxRounds) return null;

            // Use cached schedule for instant lookup
            if (cache.PlayerSchedules.TryGetValue(playerIndex, out var schedule))
            {
                var roundEntry = schedule.FirstOrDefault(s => s.Round == round);
                return roundEntry.Opponent;
            }

            return null;
        }

        public List<(int Round, Player Opponent)> GetPlayerSchedule(List<Player> players, int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= players.Count)
                return new List<(int Round, Player Opponent)>();

            var cache = GetOrCreateCache(players);

            // Return pre-computed schedule
            if (cache.PlayerSchedules.TryGetValue(playerIndex, out var schedule))
            {
                return schedule;
            }

            return new List<(int Round, Player Opponent)>();
        }

        public void InvalidateCache()
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        }
    }
}