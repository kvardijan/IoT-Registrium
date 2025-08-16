CREATE SCHEMA IF NOT EXISTS location_service;
SET search_path TO location_service;

CREATE TABLE IF NOT EXISTS location (
  id SERIAL PRIMARY KEY,
  latitude VARCHAR(100) NOT NULL,
  longitude VARCHAR(100) NOT NULL,
  address VARCHAR(200),
  description VARCHAR(200)
);

INSERT INTO location (latitude, longitude, address, description) VALUES
  ('46.16077541529039', '15.873732930368302', 'Trg Stjepana Radića 7, 49000, Krapina', 'Krapina kružni tok');
