const express = require("express");
const logger = require("../utils/logger");
const router = express.Router();

router.post("/", (req, res) => {
  const signature = req.headers["x-hub-signature-256"];

  // Log the event and payload
  const eventType = req.headers["x-github-event"];
  logger.info(`Received GitHub event: ${eventType}`, req.body);

  // Handle specific events
  if (eventType === "push") {
    const { ref, repository, pusher, commits } = req.body;

    logger.info(`Push event to branch: ${ref}`);
    logger.info(`Repository: ${repository.full_name}`);
    logger.info(`Pusher: ${pusher.name}`);

    commits.forEach((commit) => {
      logger.info(`Commit: ${commit.message} by ${commit.author.name}`);
    });
  }

  // Respond with success
  res.status(200).send("Webhook received successfully");
});

module.exports = router;
