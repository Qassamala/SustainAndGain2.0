CREATE TABLE [dbo].[UsersHistoricalTransactions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserId] NVARCHAR (450) not null foreign key references [AspNetUsers](Id),
    [CompetitionId] int not null foreign key references Competition(Id),
    [Quantity] INT not NULL,
    [TransactionPrice] money not NULL,
    [StockId] int not null foreign key references StaticStockData(Id),
    [DateTimeOfTransaction] DATETIME not null, 
    [BuyOrSell] NCHAR(4) NOT NULL
)
