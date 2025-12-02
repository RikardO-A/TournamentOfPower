import React from "react";
import { getPlayerImage, getPlayerEmoji } from "../utils/playerImages";

export default function PlayersList({ players = [], onSelect }) {
  if (!players || players.length === 0) {
    return <p>No players found.</p>;
  }

  return (
    <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
      {players.map((p, i) => {
        const playerImage = getPlayerImage(p.name || p.Name);
        const playerName = p.name || p.Name || `Player ${i}`;

        return (
          <div
            key={p.id ?? i}
            style={{
              border: "1px solid #ccc",
              padding: 8,
              width: 160,
              borderRadius: 8,
              backgroundColor: "#fff",
              boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
              transition: "transform 0.2s",
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.transform = "translateY(-4px)";
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.transform = "translateY(0)";
            }}
          >
            {playerImage ? (
              <img
                src={playerImage}
                alt={playerName}
                style={{
                  width: "100%",
                  height: 120,
                  objectFit: "cover",
                  borderRadius: 4,
                }}
                onError={(e) => {
                  // Fallback to emoji if image fails to load
                  e.target.style.display = "none";
                  e.target.nextSibling.style.display = "flex";
                }}
              />
            ) : null}
            <div
              style={{
                width: "100%",
                height: 120,
                background:
                  "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                display: playerImage ? "none" : "flex",
                alignItems: "center",
                justifyContent: "center",
                fontSize: "3em",
                borderRadius: 4,
              }}
            >
              {getPlayerEmoji(playerName)}
            </div>
            <div style={{ marginTop: 8 }}>
              <strong style={{ fontSize: "0.9em" }}>{playerName}</strong>
            </div>
            {onSelect && (
              <div style={{ marginTop: 8 }}>
                <button onClick={() => onSelect(i)} style={{ width: "100%" }}>
                  View schedule
                </button>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
