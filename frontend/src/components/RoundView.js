import React from "react";

export default function RoundView({ roundData }) {
  if (!roundData) return <p>No round data.</p>;

  const pairs = roundData.pairs ?? roundData.Pairs ?? [];

  return (
    <div>
      <h3>Round {roundData.round ?? roundData.Round}</h3>
      <ul>
        {pairs.map((p, idx) => (
          <li key={idx}>
            {(p.home ?? p.Home) || "Unknown"} vs{" "}
            {(p.away ?? p.Away) || "Unknown"}
          </li>
        ))}
      </ul>
    </div>
  );
}
