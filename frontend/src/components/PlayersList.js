import React from "react";

export default function PlayersList({ players = [], onSelect }) {
  if (!players || players.length === 0) {
    return <p>No players found.</p>;
  }

  return (
    <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
      {players.map((p, i) => (
        <div
          key={p.id ?? i}
          style={{ border: "1px solid #ccc", padding: 8, width: 160 }}
        >
          {p.image ? (
            <img
              src={p.image}
              alt={p.name}
              style={{ width: "100%", height: 80, objectFit: "cover" }}
            />
          ) : (
            <div
              style={{
                width: "100%",
                height: 80,
                background: "#eee",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              {p.name?.charAt(0) ?? "?"}
            </div>
          )}
          <div style={{ marginTop: 8 }}>
            <strong>{p.name ?? `Player ${i}`}</strong>
          </div>
          <div style={{ marginTop: 8 }}>
            <button onClick={() => onSelect(i)}>View schedule</button>
          </div>
        </div>
      ))}
    </div>
  );
}
