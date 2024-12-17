const express = require("express");
const logger = require("../utils/logger");
const router = express.Router();

router.get("/", (req, res) => {
  logger.info("Test API route hit");
  res.status(200).send("Webhook received successfully");
});

module.exports = router;
