-- =============================================
-- ANIMAL VALUE CONFIG
-- =============================================
CREATE TABLE AnimalValueConfig (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    AnimalType      NVARCHAR(100)   NOT NULL UNIQUE,

    MinAge          INT             NOT NULL,
    MaxAge          INT             NOT NULL,

    MinWeight       DECIMAL(8,2)    NOT NULL,
    MaxWeight       DECIMAL(8,2)    NOT NULL,

    MinHeight       DECIMAL(8,2)    NOT NULL,
    MaxHeight       DECIMAL(8,2)    NOT NULL,

    MinHealth       INT             NOT NULL,
    MaxHealth       INT             NOT NULL,
    Habitat 	   NVARCHAR(100)   NOT NULL,

    ImageUrlMecha       text   NULL,
    ImageUrlNormal      text   NULL,

    IsActive        BIT             NOT NULL DEFAULT 1,
    UpdatedBy       UNIQUEIDENTIFIER NULL,

    CONSTRAINT FK_AnimalValueConfig_Users FOREIGN KEY (UpdatedBy)
        REFERENCES Users(Id)
);