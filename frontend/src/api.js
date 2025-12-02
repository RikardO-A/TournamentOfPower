// api.js - wrapper for backend endpoints
const API_BASE = process.env.REACT_APP_API_URL
  ? process.env.REACT_APP_API_URL.replace(/\/*$/, "")
  : "/api";

async function request(path, opts) {
  const url = `${API_BASE}${path}`;
  const res = await fetch(url, opts);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  const contentType = res.headers.get("content-type") || "";
  if (contentType.includes("application/json")) return res.json();
  return res.text();
}

export function getPlayers() {
  return request("/player");
}

export function getRound(d) {
  return request(`/rounds/${d}`);
}

export function getPlayerSchedule(i) {
  return request(`/player/${i}/schedule`);
}

export function getMatch(i, d) {
  return request(`/match?i=${i}&d=${d}`);
}

export function getRemainingMatches(n, D) {
  return request(`/match/remaining?n=${n}&D=${D}`);
}

export function getMaxRounds(n) {
  return request(`/rounds/max?n=${n}`);
}

const api = {
  getPlayers,
  getRound,
  getPlayerSchedule,
  getMatch,
  getRemainingMatches,
  getMaxRounds,
};
export default api;
