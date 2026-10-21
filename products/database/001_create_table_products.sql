SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE dbo.Products
(
    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Products_Id DEFAULT NEWSEQUENTIALID(),
    StoreId      UNIQUEIDENTIFIER NOT NULL,
    CurrencyCode CHAR(3)          NOT NULL,
    ProductCode  NVARCHAR(50)     NOT NULL,
    Name         NVARCHAR(200)    NOT NULL,
    Description  NVARCHAR(1000)   NULL,
    Price        DECIMAL(18, 2)   NOT NULL,
    Quantity     INT              NOT NULL CONSTRAINT DF_Products_Quantity DEFAULT 0,
    IsActive     BIT              NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT 1,
    CreatedAt    DATETIME2(3)     NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt    DATETIME2(3)     NULL,
    RowVersion   ROWVERSION       NOT NULL,

    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_Products_Stores_StoreId_CurrencyCode
        FOREIGN KEY (StoreId, CurrencyCode) REFERENCES dbo.Stores (Id, CurrencyCode),

    CONSTRAINT CK_Products_ProductCode CHECK (LEN(TRIM(ProductCode)) > 0),
    CONSTRAINT CK_Products_Name        CHECK (LEN(TRIM(Name)) > 0),
    CONSTRAINT CK_Products_Price       CHECK (Price >= 0),
    CONSTRAINT CK_Products_Quantity    CHECK (Quantity >= 0)
);
GO

CREATE UNIQUE INDEX UX_Products_StoreId_ProductCode
    ON dbo.Products (StoreId, ProductCode)
    WHERE IsActive = 1;
GO

CREATE INDEX IX_Products_StoreId_CurrencyCode
    ON dbo.Products (StoreId, CurrencyCode);
GO
