CREATE TABLE [dbo].[StaticStockData]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
    [Symbol] VARCHAR(64) NOT NULL, 
    [Description] VARCHAR(1500) NULL, 
    [Sector] VARCHAR(64) NULL, 
    [CompanyName] VARCHAR(128) NULL, 
    [IsSustainable] BIT NULL
)
