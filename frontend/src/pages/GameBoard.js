import React, { useState, useEffect } from "react";
import { playMove, getTournamentStatus } from "../tournamentApi";
import { getPlayerImage, getPlayerEmoji } from "../utils/playerImages";

export default function GameBoard({ data, onPlayComplete }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [matchHistory, setMatchHistory] = useState([]);
  const [currentStatus, setCurrentStatus] = useState(data);

  useEffect(() => {
    const syncStatus = async () => {
      try {
        const status = await getTournamentStatus();
        setCurrentStatus(status);
      } catch (err) {
        console.error("Failed to sync tournament status:", err);
        setCurrentStatus(data);
      }
    };
    syncStatus();
  }, [data]);

  const playerName = currentStatus.playerName || "Player";
  const opponentName = currentStatus.opponentName || "Opponent";
  const playerWins = currentStatus.playerWins || 0;
  const opponentWins = currentStatus.opponentWins || 0;
  const roundsPlayed = currentStatus.roundsPlayed || 0;
  const isMatchComplete = currentStatus.isMatchComplete || false;
  const currentRound = currentStatus.currentRound || 1;
  const updatedScores = currentStatus.scores || {};

  const playerImage = getPlayerImage(playerName);
  const opponentImage = getPlayerImage(opponentName);

  const handleMove = async (move) => {
    setLoading(true);
    setError("");

    try {
      const response = await playMove(move);

      const roundNum =
        response.currentRoundNumber || response.CurrentRoundNumber || 0;
      const playerWinsCount = response.playerWins || response.PlayerWins || 0;
      const opponentWinsCount =
        response.opponentWins || response.OpponentWins || 0;
      const matchComplete =
        response.isMatchComplete ||
        response.IsmatchComplete ||
        playerWinsCount >= 2 ||
        opponentWinsCount >= 2;

      setMatchHistory((prev) => [
        ...prev,
        {
          round: roundNum,
          playerMove: response.playerMove || response.PlayerMove,
          opponentMove: response.opponentMove || response.OpponentMove,
          result: response.roundResult || response.RoundResult,
        },
      ]);

      setCurrentStatus({
        ...currentStatus,
        playerWins: playerWinsCount,
        opponentWins: opponentWinsCount,
        isMatchComplete: matchComplete,
        IsMatchComplete: matchComplete,
        scores:
          response.updatedScores || response.UpdatedScores || updatedScores,
      });

      setLoading(false);
    } catch (err) {
      setError(`Move failed: ${err.message}`);
      setLoading(false);
    }
  };

  const handlePlayNextMatch = () => {
    onPlayComplete(currentStatus);
  };

  return (
    <div className="game-container">
      {/* Header */}
      <div className="header-animate game-header">
        <h2 className="game-title">🎮 ROUND {currentRound}</h2>
        <p className="game-subtitle">
          Best of 3 Match •{" "}
          {isMatchComplete
            ? "✅ Match Complete"
            : "⏱️ " + (roundsPlayed + 1) + " of 3"}
        </p>
      </div>

      {/* Players */}
      <div className="players-container">
        <div className="players-flex">
          <div className="player-info">
            <div className="player-name-green">{playerName}</div>
            {playerImage ? (
              <img
                src={playerImage}
                alt={playerName}
                className="player-avatar"
                onError={(e) => {
                  e.target.style.display = "none";
                  e.target.nextSibling.style.display = "flex";
                }}
              />
            ) : null}
            <div
              className="emoji-avatar"
              style={{
                display: playerImage ? "none" : "flex",
                borderColor: "#4caf50",
              }}
            >
              {getPlayerEmoji(playerName)}
            </div>
          </div>
          <div className="vs-badge">⚔️</div>
          <div className="player-info">
            <div className="player-name-blue">
              {opponentName || "AI Opponent"}
            </div>
            {opponentImage ? (
              <img
                src={opponentImage}
                alt={opponentName}
                className="opponent-avatar"
                onError={(e) => {
                  e.target.style.display = "none";
                  e.target.nextSibling.style.display = "flex";
                }}
              />
            ) : null}
            <div
              className="emoji-avatar"
              style={{
                display: opponentImage ? "none" : "flex",
                borderColor: "#2196f3",
              }}
            >
              {getPlayerEmoji(opponentName)}
            </div>
          </div>
        </div>
      </div>

      {/* Score Display */}
      <div className="scores-grid">
        {/* Your Wins */}
        <div className="score-box score-card-green">
          <div className="score-label-green">YOUR WINS</div>
          <div className="score-number score-value-green">{playerWins}</div>
        </div>

        {/* Status */}
        <div className="status-card">
          <div className="score-label-orange">STATUS</div>
          <div className="status-value">
            {isMatchComplete
              ? playerWins > opponentWins
                ? "🎉 WIN"
                : "😢 LOSE"
              : `${roundsPlayed}/3`}
          </div>
        </div>

        {/* Opponent Wins */}
        <div className="score-box score-card-blue">
          <div className="score-label-blue">OPPONENT WINS</div>
          <div className="score-number score-value-blue">{opponentWins}</div>
        </div>
      </div>

      {/* Match History */}
      {matchHistory.length > 0 && (
        <div
          style={{
            marginBottom: 40,
            padding: 24,
            backgroundColor: "rgba(255, 255, 255, 0.95)",
            borderRadius: 16,
            border: "2px solid #e0e0e0",
            boxShadow: "0 8px 24px rgba(0, 0, 0, 0.1)",
            animation: "slideDown 0.6s ease-out 0.3s both",
          }}
        >
          <h3
            style={{
              marginTop: 0,
              marginBottom: 20,
              fontSize: "1.3em",
              color: "#333",
            }}
          >
            📋 Match History
          </h3>
          <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            {matchHistory.map((entry, idx) => (
              <div
                key={idx}
                className="history-item"
                style={{
                  padding: 14,
                  backgroundColor:
                    entry.result === "Player1" || entry.result === "Win"
                      ? "#d4edda"
                      : entry.result === "Draw"
                      ? "#fff3cd"
                      : "#f8d7da",
                  borderRadius: 10,
                  border: "2px solid #ddd",
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                  fontWeight: "600",
                  animation: `slideIn 0.4s ease-out ${idx * 0.1}s backwards`,
                }}
              >
                <span>
                  Round {entry.round}:{" "}
                  {entry.playerMove === "rock"
                    ? "🪨"
                    : entry.playerMove === "paper"
                    ? "📄"
                    : "✂️"}
                </span>
                <span style={{ opacity: 0.7 }}>vs</span>
                <span>
                  {entry.opponentMove === "rock"
                    ? "🪨"
                    : entry.opponentMove === "paper"
                    ? "📄"
                    : "✂️"}
                </span>
                <span style={{ marginLeft: "auto" }}>
                  {entry.result === "Draw"
                    ? "🤝"
                    : entry.result === "Player1" || entry.result === "Win"
                    ? "✅"
                    : "❌"}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}

      {error && (
        <div
          style={{
            marginBottom: 20,
            padding: 16,
            backgroundColor: "#ffebee",
            border: "2px solid #f44336",
            borderRadius: 10,
            color: "#c62828",
            fontWeight: "600",
            animation: "slideDown 0.3s ease-out",
          }}
        >
          ⚠️ {error}
        </div>
      )}

      {/* Action Section */}
      <div style={{ marginBottom: 24 }}>
        {!isMatchComplete && (
          <div
            style={{
              textAlign: "center",
              animation: "slideDown 0.6s ease-out",
            }}
          >
            <p
              style={{
                marginBottom: 20,
                fontSize: 18,
                color: "white",
                fontWeight: "bold",
              }}
            >
              🎯 Make Your Move:
            </p>
            <div
              style={{
                display: "flex",
                gap: 16,
                justifyContent: "center",
                flexWrap: "wrap",
              }}
            >
              {["rock", "paper", "scissors"].map((move, idx) => (
                <button
                  key={move}
                  onClick={() => handleMove(move)}
                  disabled={loading}
                  className="btn-action"
                  style={{
                    padding: "20px 40px",
                    fontSize: 20,
                    backgroundColor: loading ? "#999" : "#4caf50",
                    color: "white",
                    border: "none",
                    borderRadius: 12,
                    fontWeight: "900",
                    minWidth: "140px",
                    cursor: loading ? "not-allowed" : "pointer",
                    boxShadow: "0 8px 20px rgba(76, 175, 80, 0.3)",
                    animation: `slideDown 0.6s ease-out ${
                      0.15 + idx * 0.1
                    }s both`,
                  }}
                >
                  <div style={{ fontSize: "2em", marginBottom: 6 }}>
                    {move === "rock" ? "🪨" : move === "paper" ? "📄" : "✂️"}
                  </div>
                  {move.toUpperCase()}
                </button>
              ))}
            </div>
          </div>
        )}

        {isMatchComplete && (
          <div
            style={{
              textAlign: "center",
              animation: "slideDown 0.5s ease-out",
            }}
          >
            <div
              className="match-complete-box"
              style={{
                marginBottom: 28,
                padding: 32,
                backgroundColor:
                  playerWins > opponentWins ? "#c8e6c9" : "#ffcdd2",
                border:
                  playerWins > opponentWins
                    ? "5px solid #4caf50"
                    : "5px solid #f44336",
                borderRadius: 16,
                boxShadow:
                  playerWins > opponentWins
                    ? "0 16px 40px rgba(76, 175, 80, 0.3)"
                    : "0 16px 40px rgba(244, 67, 54, 0.3)",
              }}
            >
              <div style={{ fontSize: 56, marginBottom: 12 }}>
                {playerWins > opponentWins ? "🏆" : "🎗️"}
              </div>
              <p
                style={{
                  fontSize: 32,
                  fontWeight: "900",
                  margin: "0 0 12px 0",
                  color: playerWins > opponentWins ? "#2e7d32" : "#c62828",
                }}
              >
                {playerWins > opponentWins ? "MATCH WON!" : "MATCH LOST"}
              </p>
              <p style={{ fontSize: 16, margin: 0, color: "#555" }}>
                Final Score: {playerWins} - {opponentWins}
              </p>
            </div>

            <button
              onClick={handlePlayNextMatch}
              className="btn-action"
              style={{
                padding: "22px 50px",
                fontSize: 22,
                backgroundColor: "#2196f3",
                color: "white",
                border: "none",
                borderRadius: 12,
                fontWeight: "900",
                minWidth: "280px",
                cursor: "pointer",
                boxShadow: "0 12px 32px rgba(33, 150, 243, 0.35)",
                animation:
                  "celebrate 0.8s cubic-bezier(0.68, -0.55, 0.27, 1.55) 0.2s both",
              }}
            >
              ▶️ CONTINUE TO NEXT MATCH
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
