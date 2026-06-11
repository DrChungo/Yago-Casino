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