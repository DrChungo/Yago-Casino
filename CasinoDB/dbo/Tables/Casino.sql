-- =============================================
-- CASINO
-- =============================================
CREATE TABLE Casino (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    Name        NVARCHAR(150)   NOT NULL,
    Description NVARCHAR(500)   NULL,
    CreatedAt           NVARCHAR(50) NULL
);