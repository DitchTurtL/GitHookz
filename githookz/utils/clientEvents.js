const logger = require("./logger");

module.exports = (client) => {
  client.once("ready", () => {
    logger.info("GitHookz connected to Discord");
  });

  // Handle incoming interactions
  client.on("interactionCreate", async (interaction) => {
    if (!interaction.isCommand()) return;

    const { commandName } = interaction;

    if (commandName === "webhook") {
      const repo_url = interaction.options.getString("repository_url");

      if (repo_url) {
        logger.info("Message received:", repo_url);

        //Your repo was added. Use this URL for your webhook: ...
        await interaction.reply(repo_url);
      } else {
        await interaction.reply("You must provide a repo url!");
      }
    }
  });
};
