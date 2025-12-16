using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using ToP.Application.Interfaces;
using ToP.Application.Services;
using ToP.Domain.Classes;
using ToP.Infrastructure.Repositories;
using Xunit;

namespace ToP.Tests
{
    public class PlayerServiceDbTests
    {
        private readonly Mock<IPlayerRepository> _mockRepository;
        private readonly Mock<IRoundRobinService> _mockRoundRobinService;
        private readonly PlayerServiceDb _service;

        public PlayerServiceDbTests()
        {
            _mockRepository = new Mock<IPlayerRepository>();
            _mockRoundRobinService = new Mock<IRoundRobinService>();
            _service = new PlayerServiceDb(_mockRepository.Object, _mockRoundRobinService.Object);
        }

        [Fact]
        public async Task GetAllPlayersAsync_ShouldReturnPlayersFromRepository()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "P1" },
                new Player { Id = 2, Name = "P2" }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("P1", result[0].Name);
        }

        [Fact]
        public async Task AddPlayerAsync_ShouldAddPlayerAndInvalidateCache()
        {
            // Arrange
            var existingPlayers = new List<Player> { new Player { Id = 1, Name = "P1" } };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(existingPlayers);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Player>())).ReturnsAsync((Player p) => p);

            // Act
            var result = await _service.AddPlayerAsync("NewPlayer");

            // Assert
            Assert.Equal(2, result.Id); // Should be max ID + 1
            Assert.Equal("NewPlayer", result.Name);

            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Player>()), Times.Once);
            _mockRoundRobinService.Verify(s => s.InvalidateCache(), Times.Once);
        }
    }
}
