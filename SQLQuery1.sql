-- =============================================
-- CASINO
-- =============================================
CREATE TABLE Casino (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    Name        NVARCHAR(150)   NOT NULL,
    Description NVARCHAR(500)   NULL,
    CreatedAt           NVARCHAR(50) NULL
);

-- =============================================
-- USERS
-- =============================================
CREATE TABLE Users (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    Name            NVARCHAR(150)   NOT NULL,
    Email           NVARCHAR(255)   NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(500)   NOT NULL,
    Wallet          DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    CreatedAt       NVARCHAR(50) NULL,
    IsActive        BIT             NOT NULL DEFAULT 1
);

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

-- =============================================
-- ANIMAL SHOP
-- =============================================
CREATE TABLE AnimalShop (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    ShopName    NVARCHAR(150)   NOT NULL,
    Description NVARCHAR(500)   NULL,
    CreatedAt           NVARCHAR(50) NULL
);


-- =============================================
-- ANIMAL VALUE CONFIG
-- =============================================
CREATE TABLE AnimalValueConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    AnimalType          NVARCHAR(100)   NOT NULL UNIQUE,
    AgeMultiplier       DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    WeightMultiplier    DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    HeightMultiplier    DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    HealthMultiplier    DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    RarityMultiplier    DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    IsActive            BIT             NOT NULL DEFAULT 1,
    UpdatedBy           UNIQUEIDENTIFIER NULL,

    CONSTRAINT FK_AnimalValueConfig_Users FOREIGN KEY (UpdatedBy)
        REFERENCES Users(Id)
);

-- =============================================
-- ANIMALS
-- =============================================
CREATE TABLE Animals (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    Name                NVARCHAR(100)   NOT NULL,
    AnimalType          NVARCHAR(100)   NOT NULL,
    Rarity              NVARCHAR(50)    NOT NULL,
    Health              INT             NOT NULL DEFAULT 100,
    Age                 INT             NULL,
    Weight              DECIMAL(8,2)    NULL,
    Height              DECIMAL(8,2)    NULL,
    EstimatedValue      DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    IsAvailable         BIT             NOT NULL DEFAULT 1,
    OwnerId             UNIQUEIDENTIFIER NULL,
    AnimalValueConfigId UNIQUEIDENTIFIER NULL,
    CreatedAt           NVARCHAR(50) NULL,

    CONSTRAINT FK_Animals_Users       FOREIGN KEY (OwnerId)
        REFERENCES Users(Id)
        ON DELETE SET NULL,

    CONSTRAINT FK_Animals_ValueConfig FOREIGN KEY (AnimalValueConfigId)
        REFERENCES AnimalValueConfig(Id)
);

-- =============================================
-- SLOT GAME CONFIG
-- =============================================
CREATE TABLE SlotGameConfig (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    MachineName     NVARCHAR(100)   NOT NULL,
    Multiplier      DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    NumberOfReels   INT             NOT NULL DEFAULT 3,
    NumberOfRows    INT             NOT NULL DEFAULT 1,
    PayLines        INT             NOT NULL DEFAULT 1,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CreatedAt       NVARCHAR(50) NULL,

    GameId          UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_SlotGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);


-- =============================================
-- ROULETTE GAME CONFIG
-- =============================================
CREATE TABLE RouletteGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    TableName           NVARCHAR(100)   NOT NULL,
    RouletteType        NVARCHAR(50)    NOT NULL DEFAULT 'European',
    HasZero             BIT             NOT NULL DEFAULT 1,
    HasDoubleZero       BIT             NOT NULL DEFAULT 0,
    TotalNumbers        INT             NOT NULL DEFAULT 37,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RouletteGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_RouletteType CHECK (RouletteType IN ('European', 'American', 'French'))
);

-- =============================================
-- BLACKJACK GAME CONFIG
-- =============================================
CREATE TABLE BlackjackGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    TableName           NVARCHAR(100)   NOT NULL,
    MaxPlayers          INT             NOT NULL DEFAULT 6,
    NumberOfDecks       INT             NOT NULL DEFAULT 6,
    DealerStandsOn      INT             NOT NULL DEFAULT 17,
    BlackjackPayout     DECIMAL(4,2)    NOT NULL DEFAULT 1.50,
    AllowSplit          BIT             NOT NULL DEFAULT 1,
    AllowDoubleDown     BIT             NOT NULL DEFAULT 1,
    AllowInsurance      BIT             NOT NULL DEFAULT 1,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BlackjackGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);

-- =============================================
-- HIGHER OR LOWER GAME CONFIG
-- =============================================
CREATE TABLE HigherLowerGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    NumberOfDecks       INT             NOT NULL DEFAULT 1,
    BaseMultiplier      DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    RoundReduction      DECIMAL(8,2)    NOT NULL DEFAULT 0.50,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_HigherLowerGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);


-- =============================================
-- RUSSIAN ROULETTE GAME CONFIG
-- =============================================
CREATE TABLE RussianRouletteGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    TotalChambers       INT             NOT NULL DEFAULT 6,
    BulletCount         INT             NOT NULL DEFAULT 1,
    MaxPlayers          INT             NOT NULL DEFAULT 6,
    MinPlayers          INT             NOT NULL DEFAULT 2,
    FixedPrizePool      DECIMAL(18,2)   NOT NULL DEFAULT 500.00,
    AllowBots           BIT             NOT NULL DEFAULT 1,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RussianRouletteGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_BulletCount CHECK (BulletCount < TotalChambers)
);



-- =============================================
-- COIN FLIP RARITY CONFIG
-- =============================================
CREATE TABLE CoinFlipRarityConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    RarityName          NVARCHAR(50)    NOT NULL UNIQUE,
    MinValue            DECIMAL(18,2)   NOT NULL,
    MaxValue            DECIMAL(18,2)   NULL,
    WinProbability      DECIMAL(5,2)    NOT NULL,
    PrizeMultiplier     DECIMAL(8,2)    NOT NULL,
    IsActive            BIT             NOT NULL DEFAULT 1,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    
    CONSTRAINT FK_CoinFlipRarityConfig_Admin FOREIGN KEY (UpdatedBy)
        REFERENCES Users(Id)
);



-- =============================================
-- COIN FLIP GAME CONFIG
-- =============================================
CREATE TABLE CoinFlipGameConfig (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    ConfigName          NVARCHAR(100)   NOT NULL,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CreatedAt           NVARCHAR(50) NULL,

    GameId              UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_CoinFlipGameConfig_Games FOREIGN KEY (GameId)
        REFERENCES Games(Id)
        ON DELETE CASCADE
);

-- =============================================
-- SLOT SYMBOLS
-- =============================================
CREATE TABLE SlotSymbols (
    Id               UNIQUEIDENTIFIER PRIMARY KEY,
    SymbolName       NVARCHAR(100)   NOT NULL,
    SymbolCode       NVARCHAR(10)    NOT NULL,
    Rarity           NVARCHAR(50)    NOT NULL,
    BaseValue        DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    IsActive         BIT             NOT NULL DEFAULT 1,
    CreatedAt        NVARCHAR(50) NULL,

    SlotGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_SlotSymbols_SlotGameConfig FOREIGN KEY (SlotGameConfigId)
        REFERENCES SlotGameConfig(Id)
        ON DELETE CASCADE
);

-- =============================================
-- ROULETTE BET TYPES
-- =============================================
CREATE TABLE RouletteBetTypes (
    Id                   UNIQUEIDENTIFIER PRIMARY KEY,
    BetName              NVARCHAR(100)   NOT NULL,
    BetType              NVARCHAR(50)    NOT NULL,
    Payout               DECIMAL(8,2)    NOT NULL,
    Description          NVARCHAR(255)   NULL,
    IsActive             BIT             NOT NULL DEFAULT 1,
    CreatedAt            NVARCHAR(50) NULL,

    RouletteGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RouletteBetTypes_RouletteGameConfig FOREIGN KEY (RouletteGameConfigId)
        REFERENCES RouletteGameConfig(Id)
        ON DELETE CASCADE
);



-- =============================================
-- SHOP ANIMAL LISTINGS
-- =============================================
CREATE TABLE ShopAnimalListings (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    ListingPrice DECIMAL(18,2)   NOT NULL,
    IsSold       BIT             NOT NULL DEFAULT 0,
    ListedAt     NVARCHAR(50) NULL,
    SoldAt       NVARCHAR(50) NULL,

    AnimalId     UNIQUEIDENTIFIER NOT NULL,
    AnimalShopId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_Listings_Animals    FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_Listings_AnimalShop FOREIGN KEY (AnimalShopId)
        REFERENCES AnimalShop(Id)
);

-- =============================================
-- RUSSIAN ROULETTE LOBBIES
-- =============================================
CREATE TABLE RussianRouletteLobbies (
    Id                          UNIQUEIDENTIFIER PRIMARY KEY,
    LobbyCode                   NVARCHAR(20)    NOT NULL UNIQUE,
    Status                      NVARCHAR(20)    NOT NULL DEFAULT 'Waiting',
    CurrentPrizePool            DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    WinnerId                    UNIQUEIDENTIFIER NULL,
    CreatedAt                   NVARCHAR(50) NULL,
    StartedAt                   NVARCHAR(50) NULL,
    FinishedAt                  NVARCHAR(50) NULL,

    RussianRouletteGameConfigId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RRLobbies_GameConfig FOREIGN KEY (RussianRouletteGameConfigId)
        REFERENCES RussianRouletteGameConfig(Id),

    CONSTRAINT FK_RRLobbies_Winner FOREIGN KEY (WinnerId)
        REFERENCES Users(Id),

    CONSTRAINT CHK_LobbyStatus CHECK (Status IN ('Waiting', 'InProgress', 'Finished'))
);

-- =============================================
-- SLOT PAYOUT RULES
-- =============================================
CREATE TABLE SlotPayoutRules (
    Id               UNIQUEIDENTIFIER PRIMARY KEY,
    Combination      NVARCHAR(255)   NOT NULL,
    PayoutMultiplier DECIMAL(8,2)    NOT NULL,
    IsActive         BIT             NOT NULL DEFAULT 1,
    CreatedAt        NVARCHAR(50) NULL,

    SlotGameConfigId UNIQUEIDENTIFIER NOT NULL,
    SlotSymbolId     UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_PayoutRules_SlotGameConfig FOREIGN KEY (SlotGameConfigId)
        REFERENCES SlotGameConfig(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_PayoutRules_SlotSymbols FOREIGN KEY (SlotSymbolId)
        REFERENCES SlotSymbols(Id)
);

-- =============================================
-- GAME SESSIONS
-- =============================================
CREATE TABLE GameSessions (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    Result      NVARCHAR(10)    NOT NULL,
    MoneyEarned DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    Multiplier  DECIMAL(8,2)    NOT NULL DEFAULT 1.00,
    PlayedAt    NVARCHAR(50) NULL,

    UserId      UNIQUEIDENTIFIER NOT NULL,
    AnimalId    UNIQUEIDENTIFIER NOT NULL,
    GameId      UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_GameSessions_Users   FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_GameSessions_Animals FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_GameSessions_Games   FOREIGN KEY (GameId)
        REFERENCES Games(Id),

    CONSTRAINT CHK_Result CHECK (Result IN ('Win', 'Lose'))
);

-- =============================================
-- SHOP TRANSACTIONS
-- =============================================
CREATE TABLE ShopTransactions (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    TransactionType NVARCHAR(50)    NOT NULL,
    Amount          DECIMAL(18,2)   NOT NULL,
    TransactionDate NVARCHAR(50) NULL,

    UserId          UNIQUEIDENTIFIER NOT NULL,
    AnimalId        UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_ShopTrans_Users   FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_ShopTrans_Animals FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT CHK_TransType CHECK (TransactionType IN ('Buy', 'SellToCasino'))
);

-- =============================================
-- RUSSIAN ROULETTE PLAYERS
-- =============================================
CREATE TABLE RussianRoulettePlayers (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    IsBot        BIT             NOT NULL DEFAULT 0,
    BotName      NVARCHAR(100)   NULL,
    TurnOrder    INT             NOT NULL,
    IsAlive      BIT             NOT NULL DEFAULT 1,
    IsWinner     BIT             NOT NULL DEFAULT 0,
    EliminatedAt NVARCHAR(50) NULL,
    JoinedAt     NVARCHAR(50) NULL,

    UserId       UNIQUEIDENTIFIER NULL,
    LobbyId      UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_RRPlayers_Users   FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_RRPlayers_Lobbies FOREIGN KEY (LobbyId)
        REFERENCES RussianRouletteLobbies(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_BotOrUser CHECK (
        (IsBot = 1 AND UserId IS NULL     AND BotName IS NOT NULL) OR
        (IsBot = 0 AND UserId IS NOT NULL AND BotName IS NULL)
    )
);

-- =============================================
-- RUSSIAN ROULETTE ROUNDS
-- =============================================
CREATE TABLE RussianRouletteRounds (
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    RoundNumber  INT             NOT NULL,
    ChamberFired INT             NOT NULL,
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

-- =============================================
-- COIN FLIP SESSIONS
-- =============================================
CREATE TABLE CoinFlipSessions (
    Id                   UNIQUEIDENTIFIER PRIMARY KEY,
    CoinResult           NVARCHAR(10)    NOT NULL,
    UserChoice           NVARCHAR(10)    NOT NULL,
    IsWin                BIT             NOT NULL,
    WinProbabilityUsed   DECIMAL(5,2)    NOT NULL,
    PrizeMultiplierUsed  DECIMAL(8,2)    NOT NULL,
    MoneyEarned          DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    PlayedAt             NVARCHAR(50) NULL,

    UserId               UNIQUEIDENTIFIER NOT NULL,
    AnimalId             UNIQUEIDENTIFIER NOT NULL,
    GameSessionId        UNIQUEIDENTIFIER NOT NULL,
    CoinFlipGameConfigId UNIQUEIDENTIFIER NOT NULL,
    RarityConfigId       UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_CFSessions_Users        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_CFSessions_Animals      FOREIGN KEY (AnimalId)
        REFERENCES Animals(Id),

    CONSTRAINT FK_CFSessions_GameSessions FOREIGN KEY (GameSessionId)
        REFERENCES GameSessions(Id),

    CONSTRAINT FK_CFSessions_GameConfig   FOREIGN KEY (CoinFlipGameConfigId)
        REFERENCES CoinFlipGameConfig(Id),

    CONSTRAINT FK_CFSessions_Rarity       FOREIGN KEY (RarityConfigId)
        REFERENCES CoinFlipRarityConfig(Id),

    CONSTRAINT CHK_CoinResult CHECK (CoinResult IN ('Heads', 'Tails')),
    CONSTRAINT CHK_UserChoice CHECK (UserChoice IN ('Heads', 'Tails'))
);

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
    IsSplit            BIT             NOT NULL DEFAULT 0,

    BlackjackSessionId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJHands_Sessions FOREIGN KEY (BlackjackSessionId)
        REFERENCES BlackjackSessions(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_HandOwner CHECK (HandOwner IN ('Player', 'Dealer'))
);

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

    HandId       UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT FK_BJCards_Hands FOREIGN KEY (HandId)
        REFERENCES BlackjackHands(Id)
        ON DELETE CASCADE,

    CONSTRAINT CHK_CardSuit     CHECK (CardSuit IN ('Hearts', 'Diamonds', 'Clubs', 'Spades')),
    CONSTRAINT CHK_NumericValue CHECK (NumericValue BETWEEN 1 AND 11)
);

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

