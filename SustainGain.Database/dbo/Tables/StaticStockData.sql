CREATE TABLE [dbo].[StaticStockData]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Symbol] VARCHAR(10) NOT NULL, 
    [Description] VARCHAR(1500) NULL, 
    [Sector] VARCHAR(64) NULL, 
    [CompanyName] VARCHAR(64) NULL
)
