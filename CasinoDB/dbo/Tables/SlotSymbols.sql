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