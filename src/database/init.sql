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

	CREATE TABLE ScoreEvents (
    id UUID PRIMARY KEY,
    UserId TEXT NOT NULL,
    Score INT NOT NULL,
    Timestamp TIMESTAMP NOT NULL
);