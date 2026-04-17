CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE categories (
    Id          UUID        PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name        VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT
);