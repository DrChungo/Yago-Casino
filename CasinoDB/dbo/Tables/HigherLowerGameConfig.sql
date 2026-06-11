-- =============================================
-- HIGHER OR LOWER GAME CONFIG
-- =============================================
CREATE TABLE HigherLowerGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    BaseMultiplier      DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    RoundIncrement      DECIMAL(8,2)    NOT NULL DEFAULT 0.50,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_HigherLowerGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);