Create TABLE [dbo].[HistDataStocks]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [StockID] INT NOT NULL, 
    [DateTime] DATETIME NOT NULL, 
    [CurrentPrice] MONEY NULL,
    FOREIGN KEY (StockID) REFERENCES [StaticStockData](Id)
    
)