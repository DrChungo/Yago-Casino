-- =============================================
-- USERS
-- =============================================
CREATE TABLE Users (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    Name            NVARCHAR(150)   NOT NULL,
    Email           NVARCHAR(255)   NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(500)   NOT NULL,
    Wallet          DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    CreatedAt       NVARCHAR(50) NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    IsAdmin         BIT             NOT NULL DEFAULT 0
);