using Microsoft.AspNetCore.Mvc;
using ToP.Application.Interfaces;
using ToP.Application.DTOs;

namespace ToP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoundsController : ControllerBase
    {
        private readonly IRoundRobinService _roundRobinService;
        private readonly IPlayerService _playerService;

        public RoundsController(IRoundRobinService roundRobinService, IPlayerService playerService)
        {
            _roundRobinService = roundRobinService;
            _playerService = playerService;
        }

        /// <summary>
        /// GET /api/rounds/{d} - Returns all matches in round d (1 ? d ? n?1)
        /// </summary>
        [HttpGet("{d}")]
        public ActionResult<RoundResponse> GetRound(int d)
        {
            var players = _playerService.GetAllPlayers();
            int maxRounds = _roundRobinService.GetMaxRounds(players.Count);

            if (d < 1 || d > maxRounds)
            {
                return BadRequest(new { error = $"Round must be between 1 and {maxRounds}" });
            }

            var matches = _roundRobinService.GetMatchesForRound(players, d);
            
            var response = new RoundResponse
            {
                Round = d,
                Pairs = matches.Select(m => new PairResponse
                {
                    Home = m.Player1.Name ?? "Unknown",
                    Away = m.Player2.Name ?? "Unknown"
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// GET /api/rounds/max?n={n} - Returns max number of rounds for n participants
        /// </summary>
        [HttpGet("max")]
        public ActionResult<MaxRoundsResponse> GetMaxRounds([FromQuery] int? n)
        {
            int numberOfPlayers = n ?? _playerService.GetPlayerCount();
            
            if (numberOfPlayers < 2)
            {
                return BadRequest(new { error = "Number of players must be at least 2" });
            }

            int maxRounds = _roundRobinService.GetMaxRounds(numberOfPlayers);

            return Ok(new MaxRoundsResponse
            {
                N = numberOfPlayers,
                MaxRounds = maxRounds
            });
        }
    }
}
