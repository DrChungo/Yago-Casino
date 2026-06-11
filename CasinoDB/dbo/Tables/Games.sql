-- =============================================
-- GAMES
-- =============================================
CREATE TABLE Games (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    GameName    NVARCHAR(100)   NOT NULL,
    GameType    NVARCHAR(50)    NOT NULL,
    Description NVARCHAR(500)   NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    CONSTRAINT CHK_GameType CHECK (GameType IN (
        'Slots', 'Roulette', 'Blackjack',
        'HigherLower', 'RussianRoulette', 'CoinFlip'
    ))
);