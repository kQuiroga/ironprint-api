ALTER TABLE routine_days ADD COLUMN name          TEXT;
ALTER TABLE routine_days ADD COLUMN muscle_groups INTEGER[] NOT NULL DEFAULT '{}';
