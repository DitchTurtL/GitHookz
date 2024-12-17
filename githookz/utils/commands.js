const { SlashCommandBuilder } = require("@discordjs/builders");
const { Routes } = require("discord-api-types/v9");
const { REST } = require("@discordjs/rest");
const logger = require("./logger");

// Define slash commands
const commandsToMap = [
  new SlashCommandBuilder()
    .setName("webhook")
    .setDescription(
      "Connects a GitHub webhook to a Discord channel for real-time updates"
    )
    .addStringOption((option) =>
      option
        .setName("repository_url")
        .setDescription("The repository URL to connect the webhook to")
        .setRequired(true)
    ),
].map((command) => command.toJSON());

// Register the commands with Discord
async function registerCommands(config) {
  const rest = new REST({ version: "9" }).setToken(config.BOT_TOKEN);
  try {
    logger.info("Started refreshing application (/) commands.");

    // Register commands for a specific guild (can be used for testing)
    await rest.put(
      Routes.applicationGuildCommands(config.CLIENT_ID, config.GUILD_ID),
      {
        body: commandsToMap,
      }
    );

    logger.info("Successfully reloaded application (/) commands.");
  } catch (error) {
    logger.error("Error registering commands:", error);
  }
}

module.exports = registerCommands;
