CREATE TABLE [dbo].[StocksInCompetition]
(
	[Id] INT NOT NULL PRIMARY KEY Identity,
    [UserId] NVARCHAR(450) NOT NULL, 
    [CompId] INT NOT NULL, 
    [StockId] INT NOT NULL, 
    [Quantity] INT NOT NULL, 
    CONSTRAINT [FK_StocksInCompetition_ToTable] FOREIGN KEY ([CompId]) REFERENCES [Competition]([Id]), 
    CONSTRAINT [FK_StocksInCompetition_ToTable_1] FOREIGN KEY ([StockId]) REFERENCES [StaticStockData]([Id]), 
    CONSTRAINT [FK_StocksInCompetition_ToTable_2] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) 


)
