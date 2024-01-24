-- init.sql

\connect your_database_name;

CREATE TABLE IF NOT EXISTS "Messages" (
    "Id" SERIAL PRIMARY KEY,
    "Sender" VARCHAR(255),
    "MessageText" TEXT,
    "Timestamp" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO "Messages" ("Sender", "MessageText", "Timestamp") VALUES ('John', 'Hello, how are you?', DEFAULT);
INSERT INTO "Messages" ("Sender", "MessageText", "Timestamp") VALUES ('Alice', 'Hi John! Im doing well.', DEFAULT);