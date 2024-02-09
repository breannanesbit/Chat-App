-- init.sql

\connect your_database_name;

CREATE TABLE IF NOT EXISTS "Messages" (
    "Id" SERIAL PRIMARY KEY,
    "Sender" VARCHAR(255),
    "MessageText" TEXT,
    "ImagePath" VARCHAR(512),
    "Timestamp" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ContainerLocationId" INT
);

CREATE TABLE IF NOT EXISTS "ContainerLocation" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS "MessageContainerLocation" (
    "Id" SERIAL PRIMARY KEY,
    "MessageId" INT,
    "ContainerLocationId" INT,
    FOREIGN KEY("MessageId") REFERENCES "Messages"("Id") ON DELETE CASCADE,
    FOREIGN KEY("ContainerLocationId") REFERENCES "ContainerLocation"("Id") ON DELETE CASCADE
);

INSERT INTO "Messages" ("Sender", "MessageText", "Timestamp") VALUES ('John', 'Hello, how are you?', DEFAULT);
INSERT INTO "Messages" ("Sender", "MessageText", "Timestamp") VALUES ('Alice', 'Hi John! Im doing well.', DEFAULT);

INSERT INTO "ContainerLocation" ("Name") VALUES ('image-api-1');
INSERT INTO "ContainerLocation" ("Name") VALUES ('image-api-2');
INSERT INTO "ContainerLocation" ("Name") VALUES ('image-api-3');

