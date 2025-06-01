"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
var BASE_URL = "https://localhost:7048"; // ✅ Backend base URL
var userId = "";
var accessToken = "";
// DOM Elements
var usernameInput = document.getElementById("username");
var passwordInput = document.getElementById("password");
var loginStatus = document.getElementById("loginStatus");
var loginPanel = document.getElementById("loginPanel");
var userPanel = document.getElementById("userPanel");
var userTitle = document.getElementById("userTitle");
var alertForm = document.getElementById("alertForm");
var symbolInput = document.getElementById("symbolInput");
var priceInput = document.getElementById("priceInput");
var alertsDiv = document.getElementById("alerts");
var notificationsDiv = document.getElementById("notifications");
// Login Function
function login() {
    return __awaiter(this, void 0, void 0, function () {
        var username, password, response, result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    username = usernameInput.value;
                    password = passwordInput.value;
                    return [4 /*yield*/, fetch("".concat(BASE_URL, "/api/auth/login"), {
                            method: "POST",
                            headers: { "Content-Type": "application/json" },
                            body: JSON.stringify({ username: username, password: password }),
                        })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    result = _a.sent();
                    accessToken = result.accessToken;
                    userId = result.userId;
                    loginStatus.textContent = "Logged in";
                    loginPanel.style.display = "none";
                    userPanel.style.display = "block";
                    userTitle.textContent = "User: ".concat(username);
                    startSignalR();
                    loadAlerts();
                    return [3 /*break*/, 4];
                case 3:
                    loginStatus.textContent = "Login failed";
                    _a.label = 4;
                case 4: return [2 /*return*/];
            }
        });
    });
}
// Load User's Alerts
function loadAlerts() {
    return __awaiter(this, void 0, void 0, function () {
        var res, alerts;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, fetch("".concat(BASE_URL, "/api/alert/").concat(userId), {
                        headers: { Authorization: "Bearer ".concat(accessToken) },
                    })];
                case 1:
                    res = _a.sent();
                    if (!res.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, res.json()];
                case 2:
                    alerts = _a.sent();
                    alertsDiv.innerHTML = alerts
                        .map(function (a) { return "\u2022 ".concat(a.symbol, " \u2265 ").concat(a.priceThreshold); })
                        .join("<br/>");
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
// Handle New Alert Form Submission
alertForm.addEventListener("submit", function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var symbol, price;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                e.preventDefault();
                symbol = symbolInput.value.toUpperCase();
                price = parseFloat(priceInput.value);
                return [4 /*yield*/, fetch("".concat(BASE_URL, "/api/alert"), {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: "Bearer ".concat(accessToken),
                        },
                        body: JSON.stringify({
                            userId: userId,
                            symbol: symbol,
                            priceThreshold: price,
                            type: "Above",
                        }),
                    })];
            case 1:
                _a.sent();
                symbolInput.value = "";
                priceInput.value = "";
                loadAlerts();
                return [2 /*return*/];
        }
    });
}); });
// Setup SignalR Connection
function startSignalR() {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("".concat(BASE_URL, "/alertHub"), {
        accessTokenFactory: function () { return accessToken; },
    })
        .build();
    connection.on("AlertTriggered", function (data) {
        var msg = "\u2022 ".concat(data.symbol, " hit $").concat(data.price, " at ").concat(new Date(data.triggeredAt).toLocaleTimeString());
        var span = document.createElement("div");
        span.className = "alert";
        span.textContent = msg;
        notificationsDiv.appendChild(span);
        notificationsDiv.scrollTop = notificationsDiv.scrollHeight;
    });
    connection.start().catch(function (err) { return console.error(err.toString()); });
}
