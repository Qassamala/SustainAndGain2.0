CREATE TABLE [dbo].[UsersHistoricalTransactions]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [UserId] NVARCHAR (450) not null foreign key references [AspNetUsers](Id),
    [CompetitionId] int not null foreign key references Competition(Id),
    [Quantity] NCHAR(10) not NULL,
    [TransactionPrice] money not NULL,
    [StockId] int not null foreign key references StaticStockData(Id),
    [DateTimeOfTransaction] NCHAR(10) not null, 
    [BuyOrSell] NCHAR(10) NOT NULL
)
