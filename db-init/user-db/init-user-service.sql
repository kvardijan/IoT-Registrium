CREATE SCHEMA IF NOT EXISTS user_service;
SET search_path TO user_service;

CREATE TABLE IF NOT EXISTS "user" (
  id SERIAL PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(100) NOT NULL
);

INSERT INTO "user" (username, password) VALUES ('admin', 'admin');
INSERT INTO "user" (username, password) VALUES ('user', '$2a$12$Kl2QB/ltGmb4tRGoKdElRueMSYH41eek5dtuGT2sU9mTSlDfKHKlu');
