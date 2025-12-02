// Import existing PNG images from src
import goku from "../goku.png";
import vegeta from "../vegeta.png";
import gohan from "../gohan.png";
import frieza from "../frieza.png";
import trunks from "../trunks.png";
import madara from "../madara.png";
import ulqiorra from "../ulqiorra.png";
import aizen from "../aizen.png";
import majinbuu from "../majinbuu.png";
import orochimaru from "../orochimaru.png";
import kvothe from "../kvothe.png";
import belgarion from "../belgarion.png";

// Map player names to available character images.
export const playerImages = {
  Goku: goku,
  Vegeta: vegeta,
  Gohan: gohan,
  Frieza: frieza,
  Trunks: trunks,
  "Majin Buu": majinbuu,
  Aizen: aizen,
  Madara: madara,
  Orochimaru: orochimaru,
  Ulquiorra: ulqiorra,
  Kvothe: kvothe,
  Belgarion: belgarion,
};

// Get player image by name with fallback
export const getPlayerImage = (playerName) => {
  if (!playerName) return null;
  return playerImages[playerName] || null; // null triggers emoji fallback
};

// Get player emoji for display (used when no custom image)
export const getPlayerEmoji = (playerName) => {
  if (!playerName) return "👤";

  const emojiMap = {
    Goku: "🥋",
    Vegeta: "👑",
    Gohan: "🎓",
    Frieza: "👽",
    Trunks: "🗡️",
    "Majin Buu": "🍬",
    Aizen: "🦋",
    Madara: "👁️",
    Orochimaru: "🐍",
    Ulquiorra: "🦇",
    Kvothe: "🎶",
    Belgarion: "🐺",
  };

  return emojiMap[playerName] || "👤";
};
