-- Database: leaderboard_db

-- DROP DATABASE IF EXISTS leaderboard_db;

CREATE DATABASE leaderboard_db
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'Spanish_Colombia.1252'
    LC_CTYPE = 'Spanish_Colombia.1252'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

	CREATE TABLE scores (
    id UUID PRIMARY KEY,
    user_id TEXT NOT NULL,
    score INT NOT NULL,
    timestamp TIMESTAMP NOT NULL
);