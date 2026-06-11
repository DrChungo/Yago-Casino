-- =============================================
-- SLOT GAME CONFIG
-- =============================================
CREATE TABLE SlotGameConfig (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    MachineName     NVARCHAR(100)   NOT NULL,
    Multiplier      DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    NumberOfReels   INT             NOT NULL DEFAULT 3,
    NumberOfRows    INT             NOT NULL DEFAULT 1,
    PayLines        INT             NOT NULL DEFAULT 1,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CreatedAt       NVARCHAR(50) NULL,

    GameId          UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_SlotGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);