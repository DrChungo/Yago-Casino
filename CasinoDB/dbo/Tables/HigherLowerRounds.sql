-- =============================================
-- HIGHER OR LOWER ROUNDS
-- =============================================
CREATE TABLE HigherLowerRounds (
    Id                   UNIQUEIDENTIFIER PRIMARY KEY,
    RoundNumber          INT             NOT NULL,
    CurrentCard          NVARCHAR(10)    NOT NULL,
    NextCard             NVARCHAR(10)    NULL,
    UserGuess            NVARCHAR(10)    NOT NULL,
    IsCorrect            BIT             NULL,
    EarnedThisRound      DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    PlayedAt             NVARCHAR(50) NULL,

    HigherLowerSessionId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_HLRounds_HLSessions FOREIGN KEY (HigherLowerSessionId)
        REFERENCES HigherLowerSessions(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_UserGuess CHECK (UserGuess IN ('Higher', 'Lower'))
);