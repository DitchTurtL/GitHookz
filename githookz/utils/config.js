const logger = require("./logger");
const dotenv = require("dotenv");

dotenv.config();

function getEnvVariable(name) {
  const value = process.env[name];
  if (value === undefined) {
    logger.error(`Error: ${name} environment variable is required.`);
    process.exit(1); // Exit the app if a required variable is missing
  }
  return value;
}

module.exports = {
  CLIENT_ID: getEnvVariable("CLIENT_ID"),
  GUILD_ID: getEnvVariable("GUILD_ID"),
  BOT_TOKEN: getEnvVariable("BOT_TOKEN"),
  PORT: process.env.PORT || 3000, // Default value as fallback
};
