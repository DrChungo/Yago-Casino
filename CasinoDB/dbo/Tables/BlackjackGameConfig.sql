-- =============================================
-- BLACKJACK GAME CONFIG
-- =============================================
CREATE TABLE BlackjackGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    TableName           NVARCHAR(100)   NOT NULL,
    MaxPlayers          INT             NOT NULL DEFAULT 6,
    NumberOfDecks       INT             NOT NULL DEFAULT 6,
    DealerStandsOn      INT             NOT NULL DEFAULT 17,
    BlackjackPayout     DECIMAL(4,2)    NOT NULL DEFAULT 1.50,
    AllowDoubleDown     BIT             NOT NULL DEFAULT 1,
    AllowInsurance      BIT             NOT NULL DEFAULT 1,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BlackjackGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);