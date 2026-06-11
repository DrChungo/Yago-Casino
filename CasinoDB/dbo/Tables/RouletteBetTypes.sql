-- =============================================
-- ROULETTE BET TYPES
-- =============================================
CREATE TABLE RouletteBetTypes (
    Id                   UNIQUEIDENTIFIER PRIMARY KEY,
    BetName              NVARCHAR(100)   NOT NULL,
    Payout               DECIMAL(8,2)    NOT NULL,
    Description          NVARCHAR(255)   NULL,
    IsActive             BIT             NOT NULL DEFAULT 1,
    CreatedAt            NVARCHAR(50) NULL,

    RouletteGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RouletteBetTypes_RouletteGameConfig FOREIGN KEY (RouletteGameConfigId)
        REFERENCES RouletteGameConfig(Id)
        ON DELETE CASCADE
);