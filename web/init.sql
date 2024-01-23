-- init.sql
\connect your_database_name;

CREATE TABLE IF NOT EXISTS Messages (
    id SERIAL PRIMARY KEY,
    sender VARCHAR(255),
    message_text TEXT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
