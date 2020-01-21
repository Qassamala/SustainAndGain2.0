CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StocksID] INT NOT NULL FOREIGN KEY REFERENCES [StaticStockData](Id), 
    [DateTime] DATETIME NOT NULL, 
    [CurrentPrice] MONEY NULL
    
)