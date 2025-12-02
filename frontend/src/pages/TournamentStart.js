import React, { useState } from "react";
import { startTournament } from "../tournamentApi";

export default function TournamentStart({ onStart }) {
  const [name, setName] = useState("");
  const [playerCount, setPlayerCount] = useState(4);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleStart = async () => {
    if (!name.trim()) {
      setError("Please enter your name");
      return;
    }

    setLoading(true);
    setError("");

    try {
      const data = await startTournament(name, playerCount);
      onStart(data);
    } catch (err) {
      setError(`Failed to start tournament: ${err.message}`);
      setLoading(false);
    }
  };

  return (
    <div className="start-container">
      <h2 className="start-title">Start Tournament</h2>

      <div className="input-group">
        <label className="input-label">Your Name:</label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="ENTER NAME"
          className="input-field"
        />
      </div>

      <div className="input-group">
        <label className="input-label">Number of Players:</label>
        <select
          value={playerCount}
          onChange={(e) => setPlayerCount(Number(e.target.value))}
          className="input-field"
        >
          {[2, 4, 6, 8].map((n) => (
            <option key={n} value={n}>
              {n} PLAYERS
            </option>
          ))}
        </select>
      </div>

      {error && <p className="error-message">{error}</p>}

      <button onClick={handleStart} disabled={loading} className="btn-primary">
        {loading ? "STARTING..." : "START TOURNAMENT"}
      </button>
    </div>
  );
}
