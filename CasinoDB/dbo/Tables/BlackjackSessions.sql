-- =============================================
-- BLACKJACK SESSIONS
-- =============================================
CREATE TABLE BlackjackSessions (
    Id                    UNIQUEIDENTIFIER PRIMARY KEY,
    Status                NVARCHAR(20)    NOT NULL DEFAULT 'InProgress',
    PlayerScore           INT             NOT NULL DEFAULT 0,
    DealerScore           INT             NOT NULL DEFAULT 0,
    HasSplit              BIT             NOT NULL DEFAULT 0,
    HasDoubledDown        BIT             NOT NULL DEFAULT 0,
    HasInsurance          BIT             NOT NULL DEFAULT 0,
    InsuranceWon          BIT             NULL,
    MoneyEarned           DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    StartedAt             NVARCHAR(50) NULL,
    FinishedAt            NVARCHAR(50) NULL,

    UserId                UNIQUEIDENTIFIER NOT NULL,
    AnimalId              UNIQUEIDENTIFIER NOT NULL,
    GameSessionId         UNIQUEIDENTIFIER NOT NULL,
    BlackjackGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJSessions_Users        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_BJSessions_Animals      FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_BJSessions_GameSessions FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT FK_BJSessions_GameConfig   FOREIGN KEY (BlackjackGameConfigId)
        REFERENCES BlackjackGameConfig(Id),

    CONSTRAINT CHK_BJStatus CHECK (Status IN (
        'InProgress', 'PlayerWin', 'DealerWin', 'Push', 'Blackjack', 'Bust'
    ))
);