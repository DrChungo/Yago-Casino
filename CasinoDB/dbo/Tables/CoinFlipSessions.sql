-- =============================================
-- COIN FLIP SESSIONS
-- =============================================
CREATE TABLE CoinFlipSessions (
    Id                   UNIQUEIDENTIFIER PRIMARY KEY,
    CoinResult           NVARCHAR(10)    NOT NULL,
    UserChoice           NVARCHAR(10)    NOT NULL,
    IsWin                BIT             NOT NULL,
    WinProbabilityUsed   DECIMAL(5,2)    NOT NULL,
    PrizeMultiplierUsed  DECIMAL(8,2)    NOT NULL,
    MoneyEarned          DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    PlayedAt             NVARCHAR(50) NULL,

    UserId               UNIQUEIDENTIFIER NOT NULL,
    AnimalId             UNIQUEIDENTIFIER NOT NULL,
    GameSessionId        UNIQUEIDENTIFIER NOT NULL,
    CoinFlipGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_CFSessions_Users        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_CFSessions_Animals      FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_CFSessions_GameSessions FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT FK_CFSessions_GameConfig   FOREIGN KEY (CoinFlipGameConfigId)
        REFERENCES CoinFlipGameConfig(Id),

    CONSTRAINT CHK_CoinResult CHECK (CoinResult IN ('Heads', 'Tails')),
    CONSTRAINT CHK_UserChoice CHECK (UserChoice IN ('Heads', 'Tails'))
);