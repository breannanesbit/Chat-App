-- init.sql
CREATE USER your_username WITH PASSWORD 'your_password';

-- Create the database and set the owner
CREATE DATABASE your_database_name;
ALTER DATABASE your_database_name OWNER TO your_username;
\connect your_database_name;

CREATE TABLE IF NOT EXISTS Messages (
    id SERIAL PRIMARY KEY,
    sender VARCHAR(255),
    message_text TEXT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


INSERT INTO Messages (sender, message_text, timestamp) VALUES ('John', 'Hello, how are you?', DEFAULT);
INSERT INTO Messages (sender, message_text, timestamp) VALUES ('Alice', 'Hi John! Im doing well.', DEFAULT);