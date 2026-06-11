CREATE TABLE ActiveDrinkEffects
(
    Id               UNIQUEIDENTIFIER    NOT NULL,
    UserId           UNIQUEIDENTIFIER    NOT NULL,
    EffectType       NVARCHAR(50)        NOT NULL,
    RoundsRemaining  INT                 NOT NULL,
    CreatedAt        NVARCHAR(50)        NULL,

    CONSTRAINT PK_ActiveDrinkEffects 
        PRIMARY KEY (Id),

    CONSTRAINT FK_ActiveDrinkEffects_Users 
        FOREIGN KEY (UserId) 
        REFERENCES Users(Id) 
        ON DELETE CASCADE
);