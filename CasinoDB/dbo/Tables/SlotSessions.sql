CREATE TABLE SlotSessions (
    Id                  UNIQUEIDENTIFIER    PRIMARY KEY,
    BetAmount           DECIMAL(18,2)       NOT NULL,
    WinningSymbols      NVARCHAR(500)       NULL,

    GameSessionId       UNIQUEIDENTIFIER    NOT NULL,
    SlotGameConfigId    UNIQUEIDENTIFIER    NOT NULL,

    CONSTRAINT FK_SlotSession_GameSessions FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT FK_SlotSession_GameConfig   FOREIGN KEY (SlotGameConfigId)
        REFERENCES SlotGameConfig(Id)
);