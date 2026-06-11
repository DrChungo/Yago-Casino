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