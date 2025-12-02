using System;
using System.Collections.Generic;
using System.Linq;
using ToP.Domain.Classes;
using ToP.Application.Interfaces;
using ToP.Application.DTOs;

namespace ToP.Application.Services
{
    public interface ITournamentService
    {
        Tournament StartTournament(string playerName, int playerCount);
        TournamentStatusResponse GetStatus(Tournament tournament);
        PlayMoveResponse PlayMove(Tournament tournament, string playerMove);
        void AdvanceRound(Tournament tournament);
        FinalResultResponse GetFinal(Tournament tournament);
    }

    public class TournamentService : ITournamentService
    {
        private readonly IRoundRobinService _roundRobinService;
        private readonly IPlayerService _playerService;
        private readonly Random _random = new Random();

        public TournamentService(IRoundRobinService roundRobinService, IPlayerService playerService)
        {
            _roundRobinService = roundRobinService;
            _playerService = playerService;
        }

        public Tournament StartTournament(string playerName, int playerCount)
        {
            var players = new List<Player>();

            // Add plauer
            players.Add(new Player { Id = 0, Name = playerName, Image = string.Empty });

            // Get AI players from service
            var allAiPlayers = _playerService.GetAllPlayers();

            // Select random AI opponent
            var selectedAi = allAiPlayers
                .OrderBy(x => _random.Next())
                .Take(playerCount - 1)
                .ToList();

            // Add AI players with sequentialallly
            for (int i = 0; i < selectedAi.Count; i++)
            {
                players.Add(new Player
                {
                    Id = i + 1,
                    Name = selectedAi[i].Name,
                    Image = selectedAi[i].Image
                });
            }

            var tournament = new Tournament
            {
                Name = playerName,
                CurrentRound = 1,
                Players = players,
                CurrentPlayerIndex = 0
            };

            foreach (var player in players)
            {
                tournament.Scores[player.Id] = 0;
            }

            SetupRound(tournament, 1);
            return tournament;
        }

        private void SetupRound(Tournament tournament, int roundNumber)
        {
            var matchesForRound = _roundRobinService.GetMatchesForRound(tournament.Players, roundNumber);

            foreach (var match in matchesForRound)
            {
                var gameMatch = new GameMatch
                {
                    Round = roundNumber,
                    Player1 = match.Player1,
                    Player2 = match.Player2
                };
                tournament.Matches.Add(gameMatch);
            }
        }

        public TournamentStatusResponse GetStatus(Tournament tournament)
        {
            var currentRound = tournament.CurrentRound;
            var playerIndex = tournament.CurrentPlayerIndex!.Value;
            var match = tournament.Matches.Where(m => m.Round == currentRound)
                .FirstOrDefault(m => m.Player1.Id == playerIndex || m.Player2.Id == playerIndex);
            var opponent = match == null ? null : (match.Player1.Id == playerIndex ? match.Player2 : match.Player1);
            var maxRounds = _roundRobinService.GetMaxRounds(tournament.Players.Count);
            bool allRoundComplete = tournament.Matches.Where(m => m.Round == currentRound).All(m => m.IsComplete);
            return new TournamentStatusResponse
            {
                CurrentRound = currentRound,
                CurrentPlayerId = playerIndex,
                PlayerName = tournament.Players[playerIndex].Name ?? "Player",
                OpponentName = opponent?.Name ?? "Unknown",
                OpponentId = opponent?.Id ?? -1,
                Scores = new Dictionary<int, int>(tournament.Scores),
                PlayerWins = match == null ? 0 : (match.Player1.Id == playerIndex ? match.Player1Wins : match.Player2Wins),
                OpponentWins = match == null ? 0 : (match.Player1.Id == playerIndex ? match.Player2Wins : match.Player1Wins),
                RoundsPlayed = match?.Rounds.Count ?? 0,
                IsMatchComplete = match?.IsComplete ?? false,
                IsTournamentComplete = tournament.IsComplete,
                MaxRounds = maxRounds,
                AllRoundMatchesComplete = allRoundComplete,
                Players = tournament.Players.Select(p => new PlayerInfo { Id = p.Id, Name = p.Name ?? string.Empty, Image = p.Image ?? string.Empty }).ToList()
            };
        }

        public PlayMoveResponse PlayMove(Tournament tournament, string playerMove)
        {
            playerMove = playerMove.ToLower();
            if (!new[] { "rock", "paper", "scissors" }.Contains(playerMove))
                throw new InvalidOperationException("Invalid move");

            var currentRound = tournament.CurrentRound;
            var playerIndex = tournament.CurrentPlayerIndex!.Value;
            var playerMatch = tournament.Matches.FirstOrDefault(m =>
                m.Round == currentRound && (m.Player1.Id == playerIndex || m.Player2.Id == playerIndex) && !m.IsComplete);

            if (playerMatch == null)
                throw new InvalidOperationException("No active match for player");

            var isPlayer1 = playerMatch.Player1.Id == playerIndex;
            var opponentMove = GetAIMove();

            var roundNumber = playerMatch.Rounds.Count + 1;
            var result = DetermineRoundWinner(playerMove, opponentMove);

            playerMatch.Rounds.Add(new MatchRound
            {
                RoundNumber = roundNumber,
                Player1Move = isPlayer1 ? playerMove : opponentMove,
                Player2Move = isPlayer1 ? opponentMove : playerMove,
                Result = result
            });

            if (result == "Player1" || result == "Win")
            {
                if (isPlayer1) playerMatch.Player1Wins++;
                else playerMatch.Player2Wins++;
            }
            else if (result == "Player2" || result == "Loss")
            {
                if (isPlayer1) playerMatch.Player2Wins++;
                else playerMatch.Player1Wins++;
            }

            if (playerMatch.Player1Wins == 2 || playerMatch.Player2Wins == 2)
            {
                playerMatch.IsComplete = true;
                playerMatch.WinnerId = playerMatch.Player1Wins == 2 ? playerMatch.Player1.Id : playerMatch.Player2.Id;

                var winnerId = playerMatch.WinnerId.Value;
                if (tournament.Scores.ContainsKey(winnerId))
                    tournament.Scores[winnerId] += 3;

                if (result == "Draw")
                {
                    tournament.Scores[playerIndex] += 1;
                    tournament.Scores[playerMatch.Player1.Id == playerIndex ? playerMatch.Player2.Id : playerMatch.Player1.Id] += 1;
                }
            }

            return new PlayMoveResponse
            {
                PlayerMove = playerMove,
                OpponentMove = opponentMove,
                RoundResult = result,
                PlayerWins = isPlayer1 ? playerMatch.Player1Wins : playerMatch.Player2Wins,
                OpponentWins = isPlayer1 ? playerMatch.Player2Wins : playerMatch.Player1Wins,
                CurrentRoundNumber = roundNumber,
                IsMatchComplete = playerMatch.IsComplete,
                MatchWinnerId = playerMatch.WinnerId,
                UpdatedScores = new Dictionary<int, int>(tournament.Scores)
            };
        }

        public void AdvanceRound(Tournament tournament)
        {
            var currentRound = tournament.CurrentRound;
            var allMatchesComplete = tournament.Matches
                .Where(m => m.Round == currentRound)
                .All(m => m.IsComplete);

            if (!allMatchesComplete)
            {
                var nonPlayerMatches = tournament.Matches
                    .Where(m => m.Round == currentRound &&
                           m.Player1.Id != tournament.CurrentPlayerIndex &&
                           m.Player2.Id != tournament.CurrentPlayerIndex &&
                           !m.IsComplete)
                    .ToList();

                foreach (var match in nonPlayerMatches)
                {
                    SimulateMatchBestOf3(match);
                    var winnerId = match.WinnerId!.Value;
                    if (tournament.Scores.ContainsKey(winnerId))
                        tournament.Scores[winnerId] += 3;
                }
            }

            var maxRounds = _roundRobinService.GetMaxRounds(tournament.Players.Count);
            if (currentRound >= maxRounds)
            {
                tournament.IsComplete = true;
            }
            else
            {
                tournament.CurrentRound++;
                SetupRound(tournament, tournament.CurrentRound);
            }
        }

        private void SimulateMatchBestOf3(GameMatch match)
        {
            while (match.Player1Wins < 2 && match.Player2Wins < 2)
            {
                var move1 = GetAIMove();
                var move2 = GetAIMove();
                var result = DetermineRoundWinner(move1, move2);

                var roundNumber = match.Rounds.Count + 1;
                match.Rounds.Add(new MatchRound
                {
                    RoundNumber = roundNumber,
                    Player1Move = move1,
                    Player2Move = move2,
                    Result = result
                });

                if (result == "Player1")
                    match.Player1Wins++;
                else if (result == "Player2")
                    match.Player2Wins++;
            }

            match.IsComplete = true;
            match.WinnerId = match.Player1Wins == 2 ? match.Player1.Id : match.Player2.Id;
        }

        public FinalResultResponse GetFinal(Tournament tournament)
        {
            var winnerId = tournament.Scores.OrderByDescending(s => s.Value).First().Key;
            var winner = tournament.Players.First(p => p.Id == winnerId);
            var maxRounds = _roundRobinService.GetMaxRounds(tournament.Players.Count);

            return new FinalResultResponse
            {
                WinnerId = winnerId,
                WinnerName = winner.Name ?? "Unknown",
                FinalScores = new Dictionary<int, int>(tournament.Scores),
                TotalRounds = maxRounds
            };
        }

        private string GetAIMove()
        {
            var moves = new[] { "rock", "paper", "scissors" };
            return moves[_random.Next(moves.Length)];
        }

        private string DetermineRoundWinner(string move1, string move2)
        {
            if (move1 == move2) return "Draw";
            if ((move1 == "rock" && move2 == "scissors") ||
                (move1 == "paper" && move2 == "rock") ||
                (move1 == "scissors" && move2 == "paper"))
                return "Player1";
            return "Player2";
        }
    }
}
