import React, { useState, useEffect } from "react";
import { advanceRound, getTournamentStatus } from "../tournamentApi";

export default function Scoreboard({ data, onNext }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [currentStatus, setCurrentStatus] = useState(data);

  useEffect(() => {
    const fetchStatus = async () => {
      try {
        const status = await getTournamentStatus();
        setCurrentStatus(status);
      } catch (err) {
        console.error("Failed to fetch tournament status:", err);
      }
    };
    fetchStatus();
  }, []);

  const handleNextRound = async () => {
    setLoading(true);
    setError("");
    try {
      const result = await advanceRound();
      onNext(result);
    } catch (err) {
      setError(`Failed to advance round: ${err.message}`);
      setLoading(false);
    }
  };

  const scores = currentStatus.scores || {};
  const currentRound = currentStatus.currentRound || 1;
  const players = currentStatus.players || [];
  const nameById = Object.fromEntries(players.map((p) => [p.id, p.name]));
  const sortedScores = Object.entries(scores).sort((a, b) => b[1] - a[1]);

  return (
    <div className="scoreboard-container">
      <div className="header-animate scoreboard-header">
        <h2 className="scoreboard-title">📊 Scoreboard</h2>
        <p className="scoreboard-subtitle">Round {currentRound} Complete</p>
      </div>

      <div className="scores-list">
        <div className="scores-list-flex">
          {sortedScores.map(([playerId, points], index) => (
            <div
              key={playerId}
              className={`scorecard score-row ${
                index === 0
                  ? "border-gold"
                  : index === 1
                  ? "border-silver"
                  : index === 2
                  ? "border-bronze"
                  : "border-default"
              }`}
            >
              <div className="score-rank">
                {index === 0
                  ? "🥇"
                  : index === 1
                  ? "🥈"
                  : index === 2
                  ? "🥉"
                  : `#${index + 1}`}
              </div>
              <div className="score-name">
                {nameById[playerId] || `Player ${playerId}`}
              </div>
              <div className="score-points">{points} pts</div>
            </div>
          ))}
        </div>
      </div>

      {error && <p style={{ color: "red", textAlign: "center" }}>{error}</p>}

      <button onClick={handleNextRound} disabled={loading} className="next-btn">
        {loading ? "Loading..." : "Next Round ➡️"}
      </button>
    </div>
  );
}
