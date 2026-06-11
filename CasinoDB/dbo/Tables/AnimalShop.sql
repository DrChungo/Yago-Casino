-- =============================================
-- ANIMAL SHOP
-- =============================================
CREATE TABLE AnimalShop (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    ShopName    NVARCHAR(150)   NOT NULL,
    Description NVARCHAR(500)   NULL,
    CreatedAt           NVARCHAR(50) NULL
);