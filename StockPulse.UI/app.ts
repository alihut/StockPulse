type UserAlert = {
  symbol: string;
  targetPrice: number;
};

type UserData = {
  id: number;
  alerts: UserAlert[];
  notifications: string[];
};

const users: UserData[] = Array.from({ length: 5 }, (_, i) => ({
  id: i + 1,
  alerts: [],
  notifications: [],
}));

const usersContainer = document.getElementById("users-container")!;

function renderUI() {
  usersContainer.innerHTML = "";

  users.forEach((user) => {
    const userDiv = document.createElement("div");
    userDiv.style.width = "200px";
    userDiv.style.border = "1px solid #ccc";
    userDiv.style.padding = "10px";
    userDiv.style.fontFamily = "monospace";

    const title = document.createElement("h3");
    title.textContent = `User ${user.id}`;
    userDiv.appendChild(title);

    // Alert Form
    const alertForm = document.createElement("form");
    alertForm.innerHTML = `
      <input type="text" placeholder="Symbol" required style="width: 60px;" />
      <input type="number" placeholder="Price" required style="width: 60px;" />
      <button type="submit">Add</button>
    `;
    alertForm.onsubmit = (e) => {
      e.preventDefault();
      const inputs = alertForm.querySelectorAll("input");
      const symbol = (inputs[0] as HTMLInputElement).value.toUpperCase();
      const price = parseFloat((inputs[1] as HTMLInputElement).value);
      user.alerts.push({ symbol, targetPrice: price });
      renderUI();
    };
    userDiv.appendChild(alertForm);

    // Current Alerts
    const alertsDiv = document.createElement("div");
    alertsDiv.innerHTML = `<strong>Alerts:</strong><br/>` + 
      user.alerts.map(a => `• ${a.symbol} ≥ ${a.targetPrice}`).join("<br/>");
    userDiv.appendChild(alertsDiv);

    // Notifications
    const notifDiv = document.createElement("div");
    notifDiv.style.marginTop = "10px";
    notifDiv.style.height = "100px";
    notifDiv.style.overflowY = "auto";
    notifDiv.style.background = "#f9f9f9";
    notifDiv.style.padding = "5px";
    notifDiv.innerHTML = `<strong>Notifications:</strong><br/>` + 
      user.notifications.map(n => `<span style="color:red">• ${n}</span>`).join("<br/>");
    userDiv.appendChild(notifDiv);

    usersContainer.appendChild(userDiv);
  });
}

// Simulate Stock Prices
function simulateStockPrices() {
  const symbols = ["AAPL", "GOOG", "MSFT", "TSLA"];
  setInterval(() => {
    const symbol = symbols[Math.floor(Math.random() * symbols.length)];
    const price = Math.floor(Math.random() * 200) + 100;

    users.forEach((user) => {
      user.alerts.forEach((alert) => {
        if (alert.symbol === symbol && price >= alert.targetPrice) {
          user.notifications.push(`${symbol} hit $${price}`);
          if (user.notifications.length > 20) user.notifications.shift(); // trim
        }
      });
    });

    renderUI();
  }, 2000);
}

renderUI();
simulateStockPrices();
