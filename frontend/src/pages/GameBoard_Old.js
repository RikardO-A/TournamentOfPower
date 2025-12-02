import React, { useState, useEffect } from "react";
import { playMove, getTournamentStatus } from "../tournamentApi";

export default function GameBoard({ data, onPlayComplete }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [matchHistory, setMatchHistory] = useState([]);
  const [currentStatus, setCurrentStatus] = useState(data);

  useEffect(() => {
    // Fetch and sync fresh tournament status
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

  const playerName =
    currentStatus.playerName || currentStatus.PlayerName || "Player";
  const opponentName =
    currentStatus.opponentName || currentStatus.OpponentName || "Opponent";
  const playerWins = currentStatus.playerWins || currentStatus.PlayerWins || 0;
  const opponentWins =
    currentStatus.opponentWins || currentStatus.OpponentWins || 0;
  const roundsPlayed =
    currentStatus.roundsPlayed || currentStatus.RoundsPlayed || 0;
  const isMatchComplete =
    currentStatus.isMatchComplete || currentStatus.IsMatchComplete || false;
  const currentRound =
    currentStatus.currentRound || currentStatus.CurrentRound || 1;
  const updatedScores = currentStatus.scores || currentStatus.Scores || {};

  const handleMove = async (move) => {
    setLoading(true);
    setError("");

    try {
      const response = await playMove(move);
      console.log("PlayMove Response:", response);

      const roundNum =
        response.currentRoundNumber || response.CurrentRoundNumber || 0;
      const playerWinsCount = response.playerWins || response.PlayerWins || 0;
      const opponentWinsCount = response.opponentWins || response.OpponentWins || 0;
      const matchComplete =
        response.isMatchComplete ||
        response.IsMatchComplete ||
        playerWinsCount >= 2 ||
        opponentWinsCount >= 2;

      console.log(
        "Match State:",
        playerWinsCount,
        opponentWinsCount,
        "Complete:",
        matchComplete
      );

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
    <div style={{ maxWidth: 800, margin: "0 auto" }}>
      {/* Header with round info */}
      <div style={{ marginBottom: 24 }}>
        <h2 style={{ marginBottom: 8 }}>🎮 Tournament Round {currentRound}</h2>
        <p style={{ margin: 0, fontSize: 14, color: "#666" }}>Best of 3 Match</p>
      </div>

      {/* Players */}
      <div
        style={{
          fontSize: 18,
          marginBottom: 24,
          padding: 16,
          backgroundColor: "#e7f3ff",
          borderRadius: 4,
          border: "2px solid #0066cc",
          textAlign: "center",
        }}
      >
        <strong style={{ fontSize: 20 }}>{playerName}</strong>
        <span style={{ margin: "0 16px", fontSize: 20 }}>⚔️</span>
        <strong style={{ fontSize: 20 }}>{opponentName}</strong>
      </div>

      {/* Current Match Score - PROMINENT */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "1fr 1fr 1fr",
          gap: 16,
          marginBottom: 32,
        }}
      >
        <div
          style={{
            padding: 20,
            backgroundColor: "#e8f5e9",
            borderRadius: 8,
            textAlign: "center",
            border: "3px solid #4caf50",
          }}
        >
          <p style={{ margin: 0, fontSize: 11, color: "#555", fontWeight: "bold" }}>YOUR WINS</p>
          <p style={{ margin: "12px 0 0 0", fontSize: 48, fontWeight: "bold", color: "#4caf50" }}>
            {playerWins}
          </p>
        </div>

        <div
          style={{
            padding: 20,
            backgroundColor: "#fff8e1",
            borderRadius: 8,
            textAlign: "center",
            border: "3px solid #fbc02d",
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
          }}
        >
          <p style={{ margin: 0, fontSize: 11, color: "#555", fontWeight: "bold" }}>STATUS</p>
          <p style={{ margin: "12px 0 0 0", fontSize: 24, fontWeight: "bold", color: "#f57f17" }}>
            {isMatchComplete
              ? playerWins > opponentWins
                ? "🎉 YOU WIN!"
                : "� YOU LOSE"
              : `Round ${roundsPlayed + 1}/3`}
          </p>
        </div>

        <div
          style={{
            padding: 20,
            backgroundColor: "#e3f2fd",
            borderRadius: 8,
            textAlign: "center",
            border: "3px solid #2196f3",
          }}
        >
          <p style={{ margin: 0, fontSize: 11, color: "#555", fontWeight: "bold" }}>OPPONENT WINS</p>
          <p style={{ margin: "12px 0 0 0", fontSize: 48, fontWeight: "bold", color: "#2196f3" }}>
            {opponentWins}
          </p>
        </div>
      </div>

      {matchHistory.length > 0 && (
        <div
          style={{
            marginBottom: 24,
            padding: 16,
            backgroundColor: "#f9f9f9",
            borderRadius: 4,
            border: "1px solid #ddd",
          }}
        >
          <h3 style={{ marginTop: 0 }}>Match History</h3>
          <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
            {matchHistory.map((entry, idx) => (
              <div
                key={idx}
                style={{
                  padding: 8,
                  backgroundColor:
                    entry.result === "Player1" || entry.result === "Win"
                      ? "#d4edda"
                      : entry.result === "Draw"
                      ? "#fff3cd"
                      : "#f8d7da",
                  borderRadius: 4,
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <span>
                  <strong>Round {entry.round}:</strong> You played{" "}
                  <strong>
                    {entry.playerMove === "rock"
                      ? "🪨"
                      : entry.playerMove === "paper"
                      ? "📄"
                      : "✂️"}
                  </strong>
                </span>
                <span>
                  vs{" "}
                  <strong>
                    {entry.opponentMove === "rock"
                      ? "🪨"
                      : entry.opponentMove === "paper"
                      ? "📄"
                      : "✂️"}
                  </strong>
                </span>
                <span style={{ fontWeight: "bold" }}>
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
        <p style={{ color: "#d32f2f", marginBottom: 16, fontSize: 14, fontWeight: "bold" }}>
          ⚠️ {error}
        </p>
      )}

      {/* ACTION SECTION */}
      <div style={{ marginBottom: 24 }}>
        {!isMatchComplete && (
          <div>
            <p style={{ marginBottom: 12, fontSize: 14, color: "#555", textAlign: "center" }}>
              <strong>Make your move:</strong>
            </p>
            <div
              style={{
                display: "flex",
                gap: 12,
                justifyContent: "center",
                flexWrap: "wrap",
              }}
            >
              {["rock", "paper", "scissors"].map((move) => (
                <button
                  key={move}
                  onClick={() => handleMove(move)}
                  disabled={loading}
                  style={{
                    padding: "16px 32px",
                    fontSize: 18,
                    cursor: loading ? "not-allowed" : "pointer",
                    backgroundColor: loading ? "#ccc" : "#4caf50",
                    color: "white",
                    border: "none",
                    borderRadius: 6,
                    textTransform: "capitalize",
                    fontWeight: "bold",
                    minWidth: "120px",
                    opacity: loading ? 0.6 : 1,
                    transition: "all 0.2s",
                  }}
                  onMouseEnter={(e) => {
                    if (!loading) e.target.style.backgroundColor = "#45a049";
                  }}
                  onMouseLeave={(e) => {
                    if (!loading) e.target.style.backgroundColor = "#4caf50";
                  }}
                >
                  {move === "rock" ? "🪨" : move === "paper" ? "📄" : "✂️"}
                  <br />
                  {move}
                </button>
              ))}
            </div>
          </div>
        )}

        {isMatchComplete && (
          <div style={{ textAlign: "center" }}>
            <div
              style={{
                marginBottom: 20,
                padding: 20,
                backgroundColor: playerWins > opponentWins ? "#c8e6c9" : "#ffcdd2",
                border: playerWins > opponentWins ? "3px solid #4caf50" : "3px solid #f44336",
                borderRadius: 8,
              }}
            >
              <p style={{ fontSize: 24, fontWeight: "bold", margin: "0 0 8px 0" }}>
                {playerWins > opponentWins ? "� MATCH WON!" : "� MATCH LOST"}
              </p>
              <p style={{ fontSize: 14, margin: 0, color: "#555" }}>
                Final Score: You {playerWins} - {opponentWins} {opponentName}
              </p>
            </div>

            <p style={{ marginBottom: 16, fontSize: 14, color: "#666" }}>
              Ready for the next match?
            </p>

            <button
              onClick={handlePlayNextMatch}
              style={{
                padding: "18px 40px",
                fontSize: 18,
                cursor: "pointer",
                backgroundColor: "#2196f3",
                color: "white",
                border: "none",
                borderRadius: 6,
                fontWeight: "bold",
                minWidth: "200px",
                transition: "all 0.2s",
              }}
              onMouseEnter={(e) => {
                e.target.style.backgroundColor = "#1976d2";
              }}
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = "#2196f3";
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
