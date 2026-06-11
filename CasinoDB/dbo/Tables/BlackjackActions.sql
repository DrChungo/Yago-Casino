-- =============================================
-- BLACKJACK ACTIONS
-- =============================================
CREATE TABLE BlackjackActions (
    Id                 UNIQUEIDENTIFIER PRIMARY KEY,
    ActionType         NVARCHAR(15)    NOT NULL,
    ActionOrder        INT             NOT NULL,
    ActionAt           NVARCHAR(50) NULL,

    BlackjackSessionId UNIQUEIDENTIFIER NOT NULL,
    HandId             UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJActions_Sessions FOREIGN KEY (BlackjackSessionId)
        REFERENCES BlackjackSessions(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_BJActions_Hands FOREIGN KEY (HandId)
        REFERENCES BlackjackHands(Id),

    CONSTRAINT CHK_ActionType CHECK (ActionType IN (
        'Hit', 'Stand', 'Double', 'Split', 'Insurance'
    ))
);