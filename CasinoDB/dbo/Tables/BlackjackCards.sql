-- =============================================
-- BLACKJACK CARDS
-- =============================================
CREATE TABLE BlackjackCards (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    CardValue    NVARCHAR(5)     NOT NULL,
    CardSuit     NVARCHAR(10)    NOT NULL,
    CardDisplay  NVARCHAR(10)    NOT NULL,
    NumericValue INT             NOT NULL,
    IsFaceDown   BIT             NOT NULL DEFAULT 0,
    DealtAt      INT             NOT NULL,
    CardBlackjackValue INT             NOT NULL,


    HandId       UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJCards_Hands FOREIGN KEY (HandId)
        REFERENCES BlackjackHands(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_CardSuit     CHECK (CardSuit IN ('Hearts', 'Diamonds', 'Clubs', 'Spades')),
    CONSTRAINT CHK_NumericValue CHECK (NumericValue BETWEEN 1 AND 11)
);