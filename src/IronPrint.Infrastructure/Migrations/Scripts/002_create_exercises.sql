CREATE TABLE exercises (
    id           UUID        NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id      TEXT        NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    name         VARCHAR(200) NOT NULL,
    muscle_group INTEGER     NOT NULL,
    notes        TEXT,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX ix_exercises_user_id ON exercises (user_id);
