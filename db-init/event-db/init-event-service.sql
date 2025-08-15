CREATE SCHEMA IF NOT EXISTS event_service;
SET search_path TO event_service;

CREATE TABLE IF NOT EXISTS type (
  id SERIAL PRIMARY KEY,
  description VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS event (
  id SERIAL PRIMARY KEY,
  device VARCHAR(100),
  type INT NOT NULL,
  data JSONB,
  timestamp TIMESTAMP WITH TIME ZONE NOT NULL,
  CONSTRAINT fk_type FOREIGN KEY (type) REFERENCES type(id) ON DELETE NO ACTION ON UPDATE NO ACTION
);

INSERT INTO type (description) VALUES
  ('info update'),
  ('status change'),
  ('sent command'),
  ('firmware update'),
  ('received data'),
  ('device added'),
  ('system');

INSERT INTO event (device, type, data, timestamp) VALUES
  (NULL, 7, NULL, '2025-07-31 18:18:18+00');
