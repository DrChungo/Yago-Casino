-- =============================================
-- ROULETTE GAME CONFIG
-- =============================================
CREATE TABLE RouletteGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    TableName           NVARCHAR(100)   NOT NULL,
    RouletteType        NVARCHAR(50)    NOT NULL DEFAULT 'European',
    HasZero             BIT             NOT NULL DEFAULT 1,
    HasDoubleZero       BIT             NOT NULL DEFAULT 0,
    TotalNumbers        INT             NOT NULL DEFAULT 37,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RouletteGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_RouletteType CHECK (RouletteType IN ('European', 'American'))
);