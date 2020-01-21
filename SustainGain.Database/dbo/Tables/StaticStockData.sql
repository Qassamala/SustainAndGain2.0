CREATE TABLE [dbo].[StaticStockData]
(
	[Id] INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    [Symbol] VARCHAR(10) NOT NULL, 
    [Description] VARCHAR(1500) NULL, 
    [Sector] VARCHAR(64) NULL, 
    [CompanyName] VARCHAR(64) NULL
)
