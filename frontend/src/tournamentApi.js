const API_BASE = process.env.REACT_APP_API_URL
  ? process.env.REACT_APP_API_URL.replace(/\/*$/, "")
  : "http://localhost:5036/api"; // adjusted to backend http port

console.debug("[tournamentApi] API_BASE=", API_BASE);

async function request(path, opts = {}) {
  const url = `${API_BASE}${path}`;
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000);
  let res;
  try {
    res = await fetch(url, {
      headers: { "Content-Type": "application/json", ...opts.headers },
      signal: controller.signal,
      ...opts,
    });
  } catch (err) {
    clearTimeout(timeout);
    throw new Error(`Network error calling ${url}: ${err.message}`);
  }
  clearTimeout(timeout);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(
      `HTTP ${res.status} ${res.statusText} at ${url} :: ${text}`
    );
  }
  const contentType = res.headers.get("content-type") || "";
  return contentType.includes("application/json") ? res.json() : res.text();
}

export function startTournament(name, playerCount) {
  return request("/tournament/start", {
    method: "POST",
    body: JSON.stringify({ name, playerCount }),
  });
}

export function getTournamentStatus() {
  return request("/tournament/status");
}

export function playMove(move) {
  return request("/tournament/play", {
    method: "POST",
    body: JSON.stringify({ move }),
  });
}

export function advanceRound() {
  return request("/tournament/advance", { method: "POST" });
}

export function getFinalResult() {
  return request("/tournament/final");
}

export { API_BASE };

const api = {
  startTournament,
  getTournamentStatus,
  playMove,
  advanceRound,
  getFinalResult,
};

export default api;
