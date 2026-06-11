-- =============================================
-- RUSSIAN ROULETTE PLAYERS
-- =============================================
CREATE TABLE RussianRoulettePlayers (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    IsBot        BIT             NOT NULL DEFAULT 0,
    BotName      NVARCHAR(100)   NULL,
    TurnOrder    INT             NOT NULL,
    IsAlive      BIT             NOT NULL DEFAULT 1,
    IsWinner     BIT             NOT NULL DEFAULT 0,
    EliminatedAt NVARCHAR(50) NULL,
    JoinedAt     NVARCHAR(50) NULL,

    UserId       UNIQUEIDENTIFIER NULL,
    LobbyId      UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RRPlayers_Users   FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_RRPlayers_Lobbies FOREIGN KEY (LobbyId)
        REFERENCES RussianRouletteLobbies(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_BotOrUser CHECK (
        (IsBot = 1 AND UserId IS NULL     AND BotName IS NOT NULL) OR
        (IsBot = 0 AND UserId IS NOT NULL AND BotName IS NULL)
    )
);