CREATE TABLE workout_sessions (
    id             UUID NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id        TEXT NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    date           DATE NOT NULL,
    routine_day_id UUID REFERENCES routine_days (id) ON DELETE SET NULL,
    UNIQUE (user_id, date)
);

CREATE INDEX ix_workout_sessions_user_date ON workout_sessions (user_id, date);

CREATE TABLE exercise_logs (
    id                 UUID    NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    workout_session_id UUID    NOT NULL REFERENCES workout_sessions (id) ON DELETE CASCADE,
    exercise_id        UUID    NOT NULL REFERENCES exercises (id) ON DELETE CASCADE,
    "order"            INTEGER NOT NULL
);

CREATE TABLE set_logs (
    id              UUID    NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    exercise_log_id UUID    NOT NULL REFERENCES exercise_logs (id) ON DELETE CASCADE,
    set_number      INTEGER NOT NULL,
    weight_value    NUMERIC(8,2) NOT NULL DEFAULT 0,
    weight_unit     INTEGER NOT NULL DEFAULT 0,
    reps            INTEGER NOT NULL DEFAULT 0,
    completed       BOOLEAN NOT NULL DEFAULT FALSE
);
