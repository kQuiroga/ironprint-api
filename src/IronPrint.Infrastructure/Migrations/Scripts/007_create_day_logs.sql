CREATE TABLE day_logs (
    id      UUID    NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id TEXT    NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    date    DATE    NOT NULL,
    status  INTEGER NOT NULL DEFAULT 0,
    UNIQUE (user_id, date)
);

CREATE INDEX ix_day_logs_user_date ON day_logs (user_id, date);
