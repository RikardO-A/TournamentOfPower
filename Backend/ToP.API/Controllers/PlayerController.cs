using Microsoft.AspNetCore.Mvc;
using ToP.Application.Interfaces;
using ToP.Application.DTOs;
using ToP.Domain.Classes;

namespace ToP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IRoundRobinService _roundRobinService;
        private readonly IPlayerService _playerService;

        public PlayerController(IRoundRobinService roundRobinService, IPlayerService playerService)
        {
            _roundRobinService = roundRobinService;
            _playerService = playerService;
        }

        /// <summary>
        /// GET /api/player - Returns all players
        /// </summary>
        [HttpGet]
        public ActionResult<List<Player>> GetAllPlayers()
        {
            return Ok(_playerService.GetAllPlayers());
        }

        /// <summary>
        /// GET /api/player/{i}/schedule - Returns the complete schedule for player i over rounds 1..n?1
        /// </summary>
        [HttpGet("{i}/schedule")]
        public ActionResult<PlayerScheduleResponse> GetPlayerSchedule(int i)
        {
            var players = _playerService.GetAllPlayers();

            if (i < 0 || i >= players.Count)
            {
                return BadRequest(new { error = $"Player index must be between 0 and {players.Count - 1}" });
            }

            var player = players[i];
            var schedule = _roundRobinService.GetPlayerSchedule(players, i);

            var response = new PlayerScheduleResponse
            {
                Player = player.Name ?? "Unknown",
                N = players.Count,
                Schedule = schedule.Select(s => new ScheduleEntry
                {
                    Round = s.Round,
                    Opponent = s.Opponent.Name ?? "Unknown"
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// GET /api/player/{i}/round/{d} - Alias to direct query for player i in round d
        /// </summary>
        [HttpGet("{i}/round/{d}")]
        public ActionResult<MatchResponse> GetPlayerRound(int i, int d)
        {
            var players = _playerService.GetAllPlayers();

            if (i < 0 || i >= players.Count)
            {
                return BadRequest(new { error = $"Player index must be between 0 and {players.Count - 1}" });
            }

            int maxRounds = _roundRobinService.GetMaxRounds(players.Count);
            if (d < 1 || d > maxRounds)
            {
                return BadRequest(new { error = $"Round must be between 1 and {maxRounds}" });
            }

            var player = players[i];
            var opponent = _roundRobinService.GetOpponentForPlayerInRound(players, i, d);

            if (opponent == null)
            {
                return NotFound(new { error = "No opponent found for this player in this round" });
            }

            return Ok(new MatchResponse
            {
                Round = d,
                PlayerIndex = i,
                PlayerName = player.Name ?? "Unknown",
                Opponent = opponent.Name ?? "Unknown"
            });
        }

        /// <summary>
        /// POST /api/player - Add a new player
        /// </summary>
        [HttpPost]
        public ActionResult<Player> AddPlayer([FromBody] CreatePlayerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { error = "Player name is required" });
            }

            var player = _playerService.AddPlayer(request.Name, request.Image);
            return CreatedAtAction(nameof(GetAllPlayers), new { id = player.Id }, player);
        }

        /// <summary>
        /// DELETE /api/player/{id} - Remove a player by ID
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeletePlayer(int id)
        {
            var success = _playerService.RemovePlayer(id);
            
            if (!success)
            {
                return NotFound(new { error = $"Player with ID {id} not found" });
            }

            return NoContent();
        }
    }

    public class CreatePlayerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
