CREATE TABLE RouletteSession (
    Id                   UNIQUEIDENTIFIER PRIMARY key,
    SpinResult           INT NOT NULL,

    RouletteGameConfigId UNIQUEIDENTIFIER NOT NULL,
    GameSessionId        UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RouletteSession_RouletteGameConfig 
        FOREIGN KEY (RouletteGameConfigId) REFERENCES RouletteGameConfig(Id),

    CONSTRAINT FK_RouletteSession_GameSession 
        FOREIGN KEY (GameSessionId) REFERENCES GameSessions(Id)
);