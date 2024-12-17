const sqlite3 = require("sqlite3").verbose();
const logger = require("../utils/logger");

const db = new sqlite3.Database("./data/database.db", (err) => {
  if (err) {
    logger.error(`Error opening database: ${err.message}`);
  } else {
    logger.info("Connected to SQLite database.");

    // Create the users table if it doesn't exist
    db.run(
      `CREATE TABLE IF NOT EXISTS users (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            user_id TEXT UNIQUE NOT NULL
        )`,
      (err) => {
        if (err) {
          logger.error(`Error creating Users table: ${err.message}`);
        }
      }
    );
  }
});

module.exports = db;
