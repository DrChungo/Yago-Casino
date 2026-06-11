-- =============================================
-- GAME SESSIONS
-- =============================================
CREATE TABLE GameSessions (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    Result      NVARCHAR(10)    NOT NULL,
    MoneyEarned DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    Multiplier  DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    PlayedAt    NVARCHAR(50) NULL,

    UserId      UNIQUEIDENTIFIER NOT NULL,
    AnimalId    UNIQUEIDENTIFIER  NULL,
    GameId      UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_GameSessions_Users   FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_GameSessions_Animals FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_GameSessions_Games   FOREIGN KEY (GameId)
        REFERENCES Games(Id),

    CONSTRAINT CHK_Result CHECK (Result IN ('Win', 'Lose'))
);