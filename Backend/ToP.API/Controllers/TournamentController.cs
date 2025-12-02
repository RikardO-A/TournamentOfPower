using Microsoft.AspNetCore.Mvc;
using ToP.Application.Interfaces;
using ToP.Application.Services;
using ToP.Application.DTOs;
using ToP.Domain.Classes;

namespace ToP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : ControllerBase
    {
        private static Tournament? _currentTournament;
        private readonly ITournamentService _tournamentService;

        public TournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpPost("start")]
        public ActionResult<TournamentStatusResponse> StartTournament([FromBody] TournamentStartRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.PlayerCount < 2)
                return BadRequest(new { error = "Valid name and at least 2 players required" });

            _currentTournament = _tournamentService.StartTournament(request.Name, request.PlayerCount);
            return Ok(_tournamentService.GetStatus(_currentTournament));
        }

        [HttpGet("status")]
        public ActionResult<TournamentStatusResponse> GetStatus()
        {
            if (_currentTournament == null)
                return NotFound(new { error = "No active tournament" });

            return Ok(_tournamentService.GetStatus(_currentTournament));
        }

        [HttpPost("play")]
        public ActionResult<PlayMoveResponse> PlayMove([FromBody] PlayMoveRequest request)
        {
            if (_currentTournament == null)
                return NotFound(new { error = "No active tournament" });

            try
            {
                return Ok(_tournamentService.PlayMove(_currentTournament, request.Move));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("advance")]
        public ActionResult<TournamentStatusResponse> AdvanceRound()
        {
            if (_currentTournament == null)
                return NotFound(new { error = "No active tournament" });

            _tournamentService.AdvanceRound(_currentTournament);

            if (_currentTournament.IsComplete)
                return Ok(_tournamentService.GetFinal(_currentTournament));

            return Ok(_tournamentService.GetStatus(_currentTournament));
        }

        [HttpGet("final")]
        public ActionResult<FinalResultResponse> GetFinal()
        {
            if (_currentTournament == null || !_currentTournament.IsComplete)
                return NotFound(new { error = "Tournament not complete or no active tournament" });

            return Ok(_tournamentService.GetFinal(_currentTournament));
        }
    }
}
