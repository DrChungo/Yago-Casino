-- =============================================
-- BLACKJACK HANDS
-- =============================================
CREATE TABLE BlackjackHands (
    Id                 UNIQUEIDENTIFIER PRIMARY KEY,
    HandOwner          NVARCHAR(10)    NOT NULL,
    HandNumber         INT             NOT NULL DEFAULT 1,
    FinalScore         INT             NOT NULL DEFAULT 0,
    IsBust             BIT             NOT NULL DEFAULT 0,
    IsBlackjack        BIT             NOT NULL DEFAULT 0,

    BlackjackSessionId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJHands_Sessions FOREIGN KEY (BlackjackSessionId)
        REFERENCES BlackjackSessions(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_HandOwner CHECK (HandOwner IN ('Player', 'Dealer'))
);