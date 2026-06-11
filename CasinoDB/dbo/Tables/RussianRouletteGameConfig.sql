-- =============================================
-- RUSSIAN ROULETTE GAME CONFIG
-- =============================================
CREATE TABLE RussianRouletteGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    TotalChambers       INT             NOT NULL DEFAULT 6,
    BulletCount         INT             NOT NULL DEFAULT 1,
    MaxPlayers          INT             NOT NULL DEFAULT 6,
    MinPlayers          INT             NOT NULL DEFAULT 2,
    FixedPrizePool      DECIMAL(18,2)   NOT NULL DEFAULT 500.00,
    AllowBots           BIT             NOT NULL DEFAULT 1,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RussianRouletteGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_BulletCount CHECK (BulletCount < TotalChambers)
);