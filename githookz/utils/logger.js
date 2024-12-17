const logger = {
  info: (message, data = null) => {
    console.log(`INFO: ${message}`);
    if (data) console.log(JSON.stringify(data, null, 2));
  },
  warn: (message) => {
    console.warn(`WARN: ${message}`);
  },
  error: (message, err = null) => {
    console.error(`ERROR: ${message}`);
    if (err) console.error(err);
  },
};

module.exports = logger;
