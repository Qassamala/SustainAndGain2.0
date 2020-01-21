Create TABLE [dbo].[HistDataStocks]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StocksID] INT NOT NULL, 
    [DateTime] DATETIME NOT NULL, 
    [CurrentPrice] MONEY NULL,
    FOREIGN KEY (StocksID) REFERENCES [StaticStockData](Id)
    
)