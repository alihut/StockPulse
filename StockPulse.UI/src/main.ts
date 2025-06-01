import * as signalR from "@microsoft/signalr";

const BASE_URL = "https://localhost:7048"; // ✅ Backend base URL

let userId = "";
let accessToken = "";

// DOM Elements
const usernameInput = document.getElementById("username") as HTMLInputElement;
const passwordInput = document.getElementById("password") as HTMLInputElement;
const loginStatus = document.getElementById("loginStatus") as HTMLSpanElement;
const loginPanel = document.getElementById("loginPanel") as HTMLDivElement;
const userPanel = document.getElementById("userPanel") as HTMLDivElement;
const userTitle = document.getElementById("userTitle") as HTMLHeadingElement;
const alertForm = document.getElementById("alertForm") as HTMLFormElement;
const symbolInput = document.getElementById("symbolInput") as HTMLInputElement;
const priceInput = document.getElementById("priceInput") as HTMLInputElement;
const alertsDiv = document.getElementById("alerts") as HTMLDivElement;
const notificationsDiv = document.getElementById("notifications") as HTMLDivElement;

// Login Function
async function login() {
  const username = usernameInput.value;
  const password = passwordInput.value;

  const response = await fetch(`${BASE_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });

  if (response.ok) {
    const result = await response.json();
    accessToken = result.accessToken;
    userId = result.userId;

    loginStatus.textContent = "Logged in";
    loginPanel.style.display = "none";
    userPanel.style.display = "block";
    userTitle.textContent = `User: ${username}`;

    startSignalR();
    loadAlerts();
  } else {
    loginStatus.textContent = "Login failed";
  }
}

// Load User's Alerts
async function loadAlerts() {
  const res = await fetch(`${BASE_URL}/api/alert/${userId}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (res.ok) {
    const alerts = await res.json();
    alertsDiv.innerHTML = alerts
      .map((a: any) => `• ${a.symbol} ≥ ${a.priceThreshold}`)
      .join("<br/>");
  }
}

// Handle New Alert Form Submission
alertForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const symbol = symbolInput.value.toUpperCase();
  const price = parseFloat(priceInput.value);

  await fetch(`${BASE_URL}/api/alert`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify({
      userId,
      symbol,
      priceThreshold: price,
      type: "Above",
    }),
  });

  symbolInput.value = "";
  priceInput.value = "";
  loadAlerts();
});

// Setup SignalR Connection
function startSignalR() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BASE_URL}/alertHub`, {
      accessTokenFactory: () => accessToken,
    })
    .build();

  connection.on("AlertTriggered", (data: any) => {
    const msg = `• ${data.symbol} hit $${data.price} at ${new Date(
      data.triggeredAt
    ).toLocaleTimeString()}`;
    const span = document.createElement("div");
    span.className = "alert";
    span.textContent = msg;
    notificationsDiv.appendChild(span);
    notificationsDiv.scrollTop = notificationsDiv.scrollHeight;
  });

  connection.start().catch((err) => console.error(err.toString()));
}

document.getElementById("loginBtn")!.addEventListener("click", login);
