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