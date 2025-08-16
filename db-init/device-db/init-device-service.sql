CREATE SCHEMA IF NOT EXISTS device_service;
SET search_path TO device_service;

CREATE TABLE IF NOT EXISTS status (
  id SERIAL PRIMARY KEY,
  description VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS type (
  id SERIAL PRIMARY KEY,
  description VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS device (
  id SERIAL PRIMARY KEY,
  serial_number VARCHAR(100) NOT NULL UNIQUE,
  model VARCHAR(100) NOT NULL,
  manufacturer VARCHAR(100),
  type INTEGER NOT NULL,
  status INTEGER NOT NULL,
  firmware_version VARCHAR(45) NOT NULL,
  location INTEGER,
  last_seen TIMESTAMP WITH TIME ZONE,
  CONSTRAINT fk_status FOREIGN KEY (status) REFERENCES status(id),
  CONSTRAINT fk_type FOREIGN KEY (type) REFERENCES type(id)
);

INSERT INTO status (description) VALUES 
  ('active'),
  ('idle'),
  ('deactivated'),
  ('error');

INSERT INTO type (description) VALUES 
  ('temperature sensor'),
  ('humidity sensor'),
  ('air quality sensor'),
  ('smart trash bin'),
  ('smart traffic light'),
  ('traffic camera'),
  ('electric charging station');

INSERT INTO device (
  serial_number, model, manufacturer, type, status, firmware_version, location, last_seen
) VALUES (
  '3162281f-97da-4424-8fc0-2b15c1eb00ad', 'SuperModelx1', 'Karlo Proizvodnja d.o.o.', 1, 1, '1.0.0', '1', '2025-07-31 18:08:08+00'
);
