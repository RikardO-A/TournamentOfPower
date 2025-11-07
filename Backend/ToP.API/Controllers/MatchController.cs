using Microsoft.AspNetCore.Mvc;
using ToP.Application.Interfaces;
using ToP.Application.DTOs;

namespace ToP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IRoundRobinService _roundRobinService;
        private readonly IPlayerService _playerService;

        public MatchController(IRoundRobinService roundRobinService, IPlayerService playerService)
        {
            _roundRobinService = roundRobinService;
            _playerService = playerService;
        }

        /// <summary>
        /// GET /api/match/remaining?n={n}&D={D} - Returns number of remaining unique pairs after D rounds
        /// </summary>
        [HttpGet("remaining")]
        public ActionResult<RemainingMatchesResponse> GetRemainingMatches([FromQuery] int? n, [FromQuery] int D)
        {
            int numberOfPlayers = n ?? _playerService.GetPlayerCount();
            
            if (numberOfPlayers < 2)
            {
                return BadRequest(new { error = "Number of players must be at least 2" });
            }

            int maxRounds = _roundRobinService.GetMaxRounds(numberOfPlayers);
            if (D < 0 || D > maxRounds)
            {
                return BadRequest(new { error = $"Rounds played must be between 0 and {maxRounds}" });
            }

            int remaining = _roundRobinService.GetRemainingMatches(numberOfPlayers, D);

            return Ok(new RemainingMatchesResponse
            {
                N = numberOfPlayers,
                RoundsPlayed = D,
                RemainingMatches = remaining
            });
        }

        /// <summary>
        /// GET /api/match?n={n}&i={i}&d={d} - Returns who player i meets in round d
        /// </summary>
        [HttpGet]
        public ActionResult<MatchResponse> GetMatch([FromQuery] int? n, [FromQuery] int i, [FromQuery] int d)
        {
            var players = _playerService.GetAllPlayers();
            int numberOfPlayers = n ?? players.Count;

            if (i < 0 || i >= numberOfPlayers)
            {
                return BadRequest(new { error = $"Player index must be between 0 and {numberOfPlayers - 1}" });
            }

            int maxRounds = _roundRobinService.GetMaxRounds(numberOfPlayers);
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
    }
}
