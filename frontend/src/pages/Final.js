import React, { useEffect, useState } from "react";
import { getFinalResult } from "../tournamentApi";

export default function Final({ data, onRestart }) {
  const [finalData, setFinalData] = useState(data);

  useEffect(() => {
    const fetchFinal = async () => {
      try {
        const result = await getFinalResult();
        setFinalData(result);
      } catch (err) {
        console.error("Failed to fetch final results:", err);
      }
    };
    fetchFinal();
  }, []);

  const winnerId = finalData.winnerId || finalData.WinnerId;
  const winnerName = finalData.winnerName || finalData.WinnerName || "Unknown";
  const finalScores = finalData.finalScores || finalData.FinalScores || {};

  const sortedScores = Object.entries(finalScores).sort((a, b) => b[1] - a[1]);

  return (
    <div className="final-container">
      <h2 className="final-title">Tournament Complete! 🎉</h2>

      <div className="winner-box">
        <h3 className="winner-title">Winner: {winnerName}</h3>
        <p className="winner-score">with {finalScores[winnerId] || 0} points</p>
      </div>

      <div className="standings-section">
        <h4 className="standings-title">Final Standings</h4>
        <table className="standings-table">
          <thead>
            <tr>
              <th className="th-green-left">Pos</th>
              <th className="th-green-left">Player</th>
              <th className="th-green-right">Pts</th>
            </tr>
          </thead>
          <tbody>
            {sortedScores.map(([playerId, points], index) => (
              <tr
                key={playerId}
                className={`standings-row ${
                  index === 0
                    ? "rank-1"
                    : index === 1
                    ? "rank-2"
                    : index === 2
                    ? "rank-3"
                    : ""
                }`}
              >
                <td className="standings-cell" style={{ width: 50 }}>
                  {index === 0
                    ? "🥇"
                    : index === 1
                    ? "🥈"
                    : index === 2
                    ? "🥉"
                    : index + 1}
                </td>
                <td className="standings-cell" style={{ textAlign: "left" }}>
                  {playerId === winnerId ? "👑 " : ""}
                  {playerId}
                </td>
                <td className="standings-cell" style={{ textAlign: "right" }}>
                  {points}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <button
        onClick={onRestart}
        className="btn-primary"
        style={{ marginTop: 20 }}
      >
        PLAY AGAIN 🔄
      </button>
    </div>
  );
}
