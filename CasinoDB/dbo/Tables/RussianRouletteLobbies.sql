-- =============================================
-- RUSSIAN ROULETTE LOBBIES
-- =============================================
CREATE TABLE RussianRouletteLobbies (
    Id                          UNIQUEIDENTIFIER PRIMARY KEY,
    LobbyCode                   NVARCHAR(20)    NOT NULL UNIQUE,
    Status                      NVARCHAR(20)    NOT NULL DEFAULT 'Waiting',
    CurrentPrizePool            DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    WinnerId                    UNIQUEIDENTIFIER NULL,
    CreatedAt                   NVARCHAR(50)  NULL,
    StartedAt                   NVARCHAR(50) NULL,
    FinishedAt                  NVARCHAR(50) NULL,
    GameSessionId   UNIQUEIDENTIFIER    NULL,       -- FK al ganador via GameSessions


    RussianRouletteGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RRLobbies_GameConfig FOREIGN KEY (RussianRouletteGameConfigId)
        REFERENCES RussianRouletteGameConfig(Id),

    CONSTRAINT FK_RRLobbies_Winner FOREIGN KEY (WinnerId)
        REFERENCES Users(Id),

        CONSTRAINT FK_RussianRouletteLobbies_GameSessions -- Fk añadida
        FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT CHK_LobbyStatus CHECK (Status IN ('Waiting', 'InProgress', 'Finished'))
);