import React, { useState } from "react";
import "./App.css";
import TournamentStart from "./pages/TournamentStart";
import GameBoard from "./pages/GameBoard";
import Scoreboard from "./pages/Scoreboard";
import Final from "./pages/Final";

function App() {
  const [tournamentState, setTournamentState] = useState("start");
  const [tournamentData, setTournamentData] = useState(null);

  const handleStartTournament = (data) => {
    // Initial tournament started
    setTournamentData(data);
    setTournamentState("game");
  };

  const handlePlayComplete = (updatedData) => {
    // Player finished their match, show scoreboard
    setTournamentData(updatedData);
    setTournamentState("scoreboard");
  };

  const handleScoreboardNext = (updatedData) => {
    // Check if tournament is complete after advancing round
    if (updatedData.isTournamentComplete || updatedData.IsTournamentComplete) {
      setTournamentData(updatedData);
      setTournamentState("final");
    } else {
      // Tournament continues, go to next match
      setTournamentData(updatedData);
      setTournamentState("game");
    }
  };

  const handleRestart = () => {
    setTournamentState("start");
    setTournamentData(null);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Tournament of Power</h1>
      </header>

      <main className="app-main">
        {tournamentState === "start" && (
          <TournamentStart onStart={handleStartTournament} />
        )}

        {tournamentState === "game" && tournamentData && (
          <GameBoard
            data={tournamentData}
            onPlayComplete={handlePlayComplete}
          />
        )}

        {tournamentState === "scoreboard" && tournamentData && (
          <Scoreboard data={tournamentData} onNext={handleScoreboardNext} />
        )}

        {tournamentState === "final" && tournamentData && (
          <Final data={tournamentData} onRestart={handleRestart} />
        )}
      </main>
    </div>
  );
}

export default App;
