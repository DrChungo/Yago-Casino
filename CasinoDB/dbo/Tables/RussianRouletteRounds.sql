-- =============================================
-- RUSSIAN ROULETTE ROUNDS
-- =============================================
CREATE TABLE RussianRouletteRounds (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    RoundNumber  INT             NOT NULL,
    WasBullet    BIT             NOT NULL,
    PlayedAt     NVARCHAR(50) NULL,

    LobbyId      UNIQUEIDENTIFIER NOT NULL,
    PlayerId     UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RRRounds_Lobbies FOREIGN KEY (LobbyId)
        REFERENCES RussianRouletteLobbies(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_RRRounds_Players FOREIGN KEY (PlayerId)
        REFERENCES RussianRoulettePlayers(Id)
);