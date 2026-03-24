ALTER TABLE routines ADD COLUMN IF NOT EXISTS is_active BOOLEAN NOT NULL DEFAULT FALSE;

CREATE UNIQUE INDEX IF NOT EXISTS uix_routines_active_per_user
    ON routines (user_id)
    WHERE is_active = TRUE;
