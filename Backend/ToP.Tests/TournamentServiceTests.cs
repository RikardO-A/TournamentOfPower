using System.Collections.Generic;
using System.Linq;
using Moq;
using ToP.Application.DTOs;
using ToP.Application.Interfaces;
using ToP.Application.Services;
using ToP.Domain.Classes;
using Xunit;

namespace ToP.Tests
{
    public class TournamentServiceTests
    {
        private readonly Mock<IRoundRobinService> _mockRoundRobinService;
        private readonly Mock<IPlayerService> _mockPlayerService;
        private readonly TournamentService _service;

        public TournamentServiceTests()
        {
            _mockRoundRobinService = new Mock<IRoundRobinService>();
            _mockPlayerService = new Mock<IPlayerService>();
            _service = new TournamentService(_mockRoundRobinService.Object, _mockPlayerService.Object);
        }

        [Fact]
        public void StartTournament_ShouldInitializeTournamentCorrectly()
        {
            // Arrange
            string playerName = "Hero";
            int playerCount = 4;
            var aiPlayers = new List<Player>
            {
                new Player { Id = 10, Name = "AI1" },
                new Player { Id = 11, Name = "AI2" },
                new Player { Id = 12, Name = "AI3" }
            };

            _mockPlayerService.Setup(s => s.GetAllPlayers()).Returns(aiPlayers);

            _mockRoundRobinService.Setup(s => s.GetMatchesForRound(It.IsAny<List<Player>>(), 1))
                .Returns(new List<ToP.Domain.Classes.Match>()); // Return empty matches

            // Act
            var tournament = _service.StartTournament(playerName, playerCount);

            // Assert
            Assert.NotNull(tournament);
            Assert.Equal(playerName, tournament.Name);
            Assert.Equal(1, tournament.CurrentRound);
            Assert.Equal(playerCount, tournament.Players.Count);
            Assert.Equal(0, tournament.Players[0].Id); // Human player ID is 0
            Assert.Equal(playerName, tournament.Players[0].Name);

            // Verify AI players were added (IDs should be 1, 2, 3...)
            Assert.Equal(1, tournament.Players[1].Id);
            Assert.Equal(2, tournament.Players[2].Id);
            Assert.Equal(3, tournament.Players[3].Id);
        }

        [Fact]
        public void PlayMove_ShouldUpdateMatchState()
        {
            // Arrange
            var player = new Player { Id = 0, Name = "Hero" };
            var opponent = new Player { Id = 1, Name = "Villain" };

            var tournament = new Tournament
            {
                CurrentRound = 1,
                CurrentPlayerIndex = 0,
                Players = new List<Player> { player, opponent },
                Scores = new Dictionary<int, int> { { 0, 0 }, { 1, 0 } }
            };

            var match = new GameMatch
            {
                Round = 1,
                Player1 = player,
                Player2 = opponent,
                IsComplete = false,
                Rounds = new List<MatchRound>()
            };
            tournament.Matches.Add(match);

            // Act
            var response = _service.PlayMove(tournament, "rock");

            // Assert
            Assert.NotNull(response);
            Assert.Equal("rock", response.PlayerMove);
            Assert.NotNull(response.OpponentMove);
            Assert.Contains(response.RoundResult, new[] { "Player1", "Player2", "Draw" });

            var updatedMatch = tournament.Matches.First();
            Assert.Single(updatedMatch.Rounds);
        }

        [Fact]
        public void AdvanceRound_ShouldSimulateOtherMatches_AndAdvanceRound()
        {
            // Arrange
            var player = new Player { Id = 0, Name = "Hero" };
            var ai1 = new Player { Id = 1, Name = "AI1" };
            var ai2 = new Player { Id = 2, Name = "AI2" };
            var ai3 = new Player { Id = 3, Name = "AI3" };

            var tournament = new Tournament
            {
                CurrentRound = 1,
                CurrentPlayerIndex = 0,
                Players = new List<Player> { player, ai1, ai2, ai3 },
                Scores = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } }
            };

            // Player match (complete)
            var playerMatch = new GameMatch
            {
                Round = 1,
                Player1 = player,
                Player2 = ai1,
                IsComplete = true,
                WinnerId = 0,
                Player1Wins = 2
            };

            // AI match (incomplete)
            var aiMatch = new GameMatch
            {
                Round = 1,
                Player1 = ai2,
                Player2 = ai3,
                IsComplete = false,
                Rounds = new List<MatchRound>()
            };

            tournament.Matches.Add(playerMatch);
            tournament.Matches.Add(aiMatch);

            _mockRoundRobinService.Setup(s => s.GetMaxRounds(4)).Returns(3);
            _mockRoundRobinService.Setup(s => s.GetMatchesForRound(It.IsAny<List<Player>>(), 2))
                .Returns(new List<ToP.Domain.Classes.Match>()); // Setup for next round

            // Act
            _service.AdvanceRound(tournament);

            // Assert
            Assert.True(aiMatch.IsComplete); // AI match should be simulated
            Assert.NotNull(aiMatch.WinnerId);
            Assert.Equal(2, tournament.CurrentRound); // Should advance to round 2
        }
    }
}
