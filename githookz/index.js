const { Client, GatewayIntentBits } = require("discord.js");
const { Routes } = require("discord-api-types/v9");

const logger = require("./utils/logger");
const express = require("express"); // Express server for incoming webhooks from Github

const config = require("./utils/config"); // Configuration from environment variables
const webhookRoute = require("./routes/webhookRoute"); // Configure webhook route
const testApiRoute = require("./routes/testApiRoute");

const registerCommands = require("./utils/commands"); // Register interactions with Discord
const setupEvents = require("./utils/clientEvents"); // Register Bot events

const app = express(); // Create the Express app
app.use(express.json({ limit: "5mb" }));
app.use("/webhook", webhookRoute); // Webhook endpoint
app.use("/testApi", testApiRoute);

// Start the express server
app.listen(config.PORT, () => {
  logger.info(`Backend running at ${config.BASE_URL}:${config.PORT}`);
});

// Create the bot client
const client = new Client({
  intents: [GatewayIntentBits.Guilds],
});

// Register the bot events
setupEvents(client);

// Start the bot and register the commands
(async () => {
  await registerCommands(config);
  client.login(config.BOT_TOKEN);
})();

const db = require("./utils/database");
