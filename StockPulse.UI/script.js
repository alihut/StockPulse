    const baseUrl = "https://localhost:7048/api";
    let userToken = null;
    let adminToken = null;
    let userConnection = null;

    async function loginUser() {
      const username = document.getElementById("user-username").value;
      const password = document.getElementById("user-password").value;
      const res = await fetch(`${baseUrl}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
      });
      const data = await res.json();
      userToken = data.token;
      document.getElementById("user-login").style.display = "none";
      document.getElementById("user-actions").style.display = "block";
      document.getElementById("logout-btn").style.display = "inline";
      await connectToSignalR();
      await getAlerts();
    }

    async function connectToSignalR() {
      userConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7048/alerts", {
          accessTokenFactory: () => userToken
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      userConnection.on("AlertTriggered", alert => {
        const log = document.getElementById("user-log");
        log.textContent += `\n🔔 Alert: ${JSON.stringify(alert)}`;
        getAlerts();
      });

      await userConnection.start();
      document.getElementById("user-log").textContent += `\n✅ Connected to SignalR hub.`;
    }

    async function registerAlert() {
      const symbol = document.getElementById("user-symbol").value;
      const threshold = parseFloat(document.getElementById("user-threshold").value);
      const alertType = parseInt(document.getElementById("user-alert-type").value);

      const res = await fetch(`${baseUrl}/alert`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${userToken}`
        },
        body: JSON.stringify({
          Symbol: symbol,
          PriceThreshold: threshold,
          Type: alertType
        })
      });

      if (res.ok) {
        alert("✅ Alert registered.");
        getAlerts();
      } else {
        alert("❌ Failed to register alert.");
      }
    }

    async function getAlerts() {
      const res = await fetch(`${baseUrl}/alert`, {
        headers: { "Authorization": `Bearer ${userToken}` }
      });
      const alerts = await res.json();
      document.getElementById("user-alerts").textContent = JSON.stringify(alerts, null, 2);
    }

    async function loginAdmin() {
      const username = document.getElementById("admin-username").value;
      const password = document.getElementById("admin-password").value;
      const res = await fetch(`${baseUrl}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
      });
      const data = await res.json();
      adminToken = data.token;
      document.getElementById("admin-login").style.display = "none";
      document.getElementById("admin-actions").style.display = "block";
    }

    async function simulatePrice() {
      const symbol = document.getElementById("admin-symbol").value;
      const price = parseFloat(document.getElementById("admin-price").value);
      const res = await fetch(`${baseUrl}/stockprice`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${adminToken}`
        },
        body: JSON.stringify({ symbol, price })
      });
      const log = document.getElementById("admin-log");
      if (res.ok) {
        log.textContent += `\n✅ Submitted: ${symbol} = ${price}`;
      } else {
        log.textContent += `\n❌ Error submitting price`;
      }
    }

    function logoutUser() {
      userToken = null;
      userConnection?.stop();
      document.getElementById("user-actions").style.display = "none";
      document.getElementById("user-log").textContent = "🔔 Waiting for alerts...";
      document.getElementById("user-alerts").textContent = "[ ]";
      document.getElementById("user-login").style.display = "block";
      document.getElementById("logout-btn").style.display = "none";
    }