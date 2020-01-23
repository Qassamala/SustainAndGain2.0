CREATE TABLE [dbo].[StaticStockData] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Symbol]        VARCHAR (64)   NOT NULL,
    [Description]   VARCHAR (1500) NULL,
    [Sector]        VARCHAR (64)   NULL,
    [CompanyName]   VARCHAR (128)  NULL,
    [IsSustainable] BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

