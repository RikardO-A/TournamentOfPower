using System.Collections.Generic;
using System.Linq;
using ToP.Application.Services;
using ToP.Domain.Classes;
using Xunit;

namespace ToP.Tests
{
    public class RoundRobinServiceTests
    {
        private readonly RoundRobinService _service;

        public RoundRobinServiceTests()
        {
            _service = new RoundRobinService();
        }

        [Theory]
        [InlineData(4, 3)] // 4 players -> 3 rounds
        [InlineData(3, 3)] // 3 players -> 4 (with bye) -> 3 rounds
        [InlineData(2, 1)] // 2 players -> 1 round
        public void GetMaxRounds_ShouldReturnCorrectNumberOfRounds(int playerCount, int expectedRounds)
        {
            // Act
            var result = _service.GetMaxRounds(playerCount);

            // Assert
            Assert.Equal(expectedRounds, result);
        }

        [Fact]
        public void GetMatchesForRound_ShouldReturnCorrectPairings_For4Players()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "P1" },
                new Player { Id = 2, Name = "P2" },
                new Player { Id = 3, Name = "P3" },
                new Player { Id = 4, Name = "P4" }
            };

            // Act
            var round1Matches = _service.GetMatchesForRound(players, 1);

            // Assert
            // Round 1: (1 vs 4), (2 vs 3)
            Assert.Equal(2, round1Matches.Count);

            var allPlayersInMatches = new List<int>();
            foreach (var match in round1Matches)
            {
                allPlayersInMatches.Add(match.Player1.Id);
                allPlayersInMatches.Add(match.Player2.Id);
            }

            Assert.Contains(1, allPlayersInMatches);
            Assert.Contains(2, allPlayersInMatches);
            Assert.Contains(3, allPlayersInMatches);
            Assert.Contains(4, allPlayersInMatches);
        }

        [Fact]
        public void GetMatchesForRound_ShouldHandleOddNumberOfPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "P1" },
                new Player { Id = 2, Name = "P2" },
                new Player { Id = 3, Name = "P3" }
            };

            // Act
            var round1Matches = _service.GetMatchesForRound(players, 1);

            // Assert
            // 3 players -> 1 bye -> 1 match per round (plus one player sitting out)
            Assert.Single(round1Matches);
        }
    }
}
