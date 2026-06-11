-- =============================================
-- COIN FLIP GAME CONFIG
-- =============================================
CREATE TABLE CoinFlipGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_CoinFlipGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);