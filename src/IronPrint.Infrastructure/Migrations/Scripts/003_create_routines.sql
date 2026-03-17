CREATE TABLE routines (
    id             UUID         NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id        TEXT         NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    name           VARCHAR(200) NOT NULL,
    weeks_duration INTEGER      NOT NULL,
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE INDEX ix_routines_user_id ON routines (user_id);

CREATE TABLE routine_days (
    id          UUID    NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    routine_id  UUID    NOT NULL REFERENCES routines (id) ON DELETE CASCADE,
    day_of_week INTEGER NOT NULL,
    UNIQUE (routine_id, day_of_week)
);

CREATE TABLE routine_exercises (
    id             UUID    NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    routine_day_id UUID    NOT NULL REFERENCES routine_days (id) ON DELETE CASCADE,
    exercise_id    UUID    NOT NULL REFERENCES exercises (id) ON DELETE CASCADE,
    "order"        INTEGER NOT NULL,
    target_sets    INTEGER NOT NULL,
    target_reps    INTEGER NOT NULL
);
