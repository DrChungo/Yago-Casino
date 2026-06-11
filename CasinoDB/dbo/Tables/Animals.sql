-- =============================================
-- ANIMALS
-- =============================================
CREATE TABLE Animals (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY,
    Name                NVARCHAR(100)   NOT NULL,
    AnimalType          NVARCHAR(100)   NOT NULL,
    Rarity              BIT   NOT NULL,
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