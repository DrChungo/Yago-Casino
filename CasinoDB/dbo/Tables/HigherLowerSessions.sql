-- =============================================
-- HIGHER OR LOWER SESSIONS
-- =============================================
CREATE TABLE HigherLowerSessions (
    Id                      UNIQUEIDENTIFIER PRIMARY KEY,
    Status                  NVARCHAR(20)    NOT NULL DEFAULT 'InProgress',
    TotalEarned             DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    RoundsPlayed            INT             NOT NULL DEFAULT 0,
    StartedAt               NVARCHAR(50) NULL,
    FinishedAt              NVARCHAR(50) NULL,

    UserId                  UNIQUEIDENTIFIER NOT NULL,
    AnimalId                UNIQUEIDENTIFIER NOT NULL,
    GameSessionId           UNIQUEIDENTIFIER NOT NULL,
    HigherLowerGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_HLSessions_Users        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_HLSessions_Animals      FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_HLSessions_GameSessions FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT FK_HLSessions_GameConfig   FOREIGN KEY (HigherLowerGameConfigId)
        REFERENCES HigherLowerGameConfig(Id),

    CONSTRAINT CHK_HLStatus CHECK (Status IN ('InProgress', 'Cashed', 'Lost'))
);