-- init.sql
CREATE TABLE IF NOT EXISTS messages (
    id SERIAL PRIMARY KEY,
    sender VARCHAR(255),
    message_text TEXT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
