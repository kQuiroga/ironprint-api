CREATE TABLE users (
    id          TEXT        NOT NULL PRIMARY KEY,
    username    VARCHAR(256),
    normalized_username VARCHAR(256),
    email       VARCHAR(256),
    normalized_email    VARCHAR(256),
    email_confirmed     BOOLEAN     NOT NULL DEFAULT FALSE,
    password_hash       TEXT,
    security_stamp      TEXT,
    concurrency_stamp   TEXT,
    phone_number        TEXT,
    phone_number_confirmed  BOOLEAN NOT NULL DEFAULT FALSE,
    two_factor_enabled      BOOLEAN NOT NULL DEFAULT FALSE,
    lockout_end         TIMESTAMPTZ,
    lockout_enabled     BOOLEAN     NOT NULL DEFAULT FALSE,
    access_failed_count INTEGER     NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX ix_users_normalized_username ON users (normalized_username) WHERE normalized_username IS NOT NULL;
CREATE UNIQUE INDEX ix_users_normalized_email    ON users (normalized_email)    WHERE normalized_email    IS NOT NULL;

CREATE TABLE roles (
    id                TEXT NOT NULL PRIMARY KEY,
    name              VARCHAR(256),
    normalized_name   VARCHAR(256),
    concurrency_stamp TEXT
);

CREATE UNIQUE INDEX ix_roles_normalized_name ON roles (normalized_name) WHERE normalized_name IS NOT NULL;

CREATE TABLE user_roles (
    user_id TEXT NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    role_id TEXT NOT NULL REFERENCES roles (id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);

CREATE TABLE user_claims (
    id          SERIAL PRIMARY KEY,
    user_id     TEXT NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    claim_type  TEXT,
    claim_value TEXT
);

CREATE TABLE user_logins (
    login_provider          TEXT NOT NULL,
    provider_key            TEXT NOT NULL,
    provider_display_name   TEXT,
    user_id                 TEXT NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    PRIMARY KEY (login_provider, provider_key)
);

CREATE TABLE user_tokens (
    user_id        TEXT NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    login_provider TEXT NOT NULL,
    name           TEXT NOT NULL,
    value          TEXT,
    PRIMARY KEY (user_id, login_provider, name)
);

CREATE TABLE role_claims (
    id          SERIAL PRIMARY KEY,
    role_id     TEXT NOT NULL REFERENCES roles (id) ON DELETE CASCADE,
    claim_type  TEXT,
    claim_value TEXT
);
