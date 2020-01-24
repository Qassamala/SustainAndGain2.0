CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [StockId] INT NOT NULL REFERENCES [dbo].[StaticStockData] ([Id]), 
    [OrderValue] MONEY NOT NULL, 
    [TimeOfInsertion] DATETIME NOT NULL, 
    [BuyOrSell] BIT NOT NULL, 
    [UserId] NVARCHAR(450) NOT NULL REFERENCES [dbo].[AspNetUsers] ([Id]), 
    [CompId] INT NOT NULL REFERENCES [dbo].[Competition] ([Id])
)
