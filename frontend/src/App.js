import React, { useEffect, useState } from "react";
import "./App.css";
import { getPlayers, getRound, getPlayerSchedule, getMaxRounds } from "./api";
import PlayersList from "./components/PlayersList";
import RoundView from "./components/RoundView";

function App() {
  const [players, setPlayers] = useState([]);
  const [selectedPlayerIndex, setSelectedPlayerIndex] = useState(null);
  const [playerSchedule, setPlayerSchedule] = useState(null);
  const [round, setRound] = useState(1);
  const [roundData, setRoundData] = useState(null);
  const [maxRounds, setMaxRounds] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        const p = await getPlayers();
        setPlayers(p);

        // fetch max rounds for current number of players
        const res = await getMaxRounds(p.length);
        setMaxRounds(res.maxRounds ?? res.MaxRounds ?? null);
      } catch (err) {
        console.error("Failed to load players", err);
      }
    }

    load();
  }, []);

  useEffect(() => {
    async function loadRound() {
      try {
        const r = await getRound(round);
        setRoundData(r);
      } catch (err) {
        console.error("Failed to load round", err);
        setRoundData(null);
      }
    }

    loadRound();
  }, [round]);

  async function showPlayerSchedule(i) {
    setSelectedPlayerIndex(i);
    try {
      const sched = await getPlayerSchedule(i);
      setPlayerSchedule(sched);
    } catch (err) {
      console.error("Failed to load schedule", err);
      setPlayerSchedule(null);
    }
  }

  return (
    <div className="App">
      <header className="App-header">
        <h1>Tournament of Power — Frontend</h1>
      </header>

      <main style={{ padding: 16 }}>
        <section style={{ marginBottom: 24 }}>
          <h2>Players</h2>
          <PlayersList
            players={players}
            onSelect={(i) => showPlayerSchedule(i)}
          />
        </section>

        <section style={{ marginBottom: 24 }}>
          <h2>Selected player schedule</h2>
          {selectedPlayerIndex === null && (
            <p>Select a player to view schedule.</p>
          )}
          {playerSchedule && (
            <div>
              <h3>{playerSchedule.player ?? playerSchedule.Player}</h3>
              <p>Participants: {playerSchedule.n ?? playerSchedule.N}</p>
              <ol>
                {(playerSchedule.schedule ?? playerSchedule.Schedule ?? []).map(
                  (s, idx) => (
                    <li key={idx}>
                      Round {s.round ?? s.Round}: {s.opponent ?? s.Opponent}
                    </li>
                  )
                )}
              </ol>
            </div>
          )}
        </section>

        <section>
          <h2>Round viewer</h2>
          <div style={{ marginBottom: 8 }}>
            <label>Round: </label>
            <input
              type="number"
              min={1}
              max={maxRounds || 100}
              value={round}
              onChange={(e) => setRound(Number(e.target.value))}
            />
            <span style={{ marginLeft: 8 }}>
              Max rounds: {maxRounds ?? "—"}
            </span>
          </div>

          {roundData ? (
            <RoundView roundData={roundData} />
          ) : (
            <p>No data for this round.</p>
          )}
        </section>
      </main>
    </div>
  );
}

export default App;
