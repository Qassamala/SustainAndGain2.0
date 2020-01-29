CREATE TABLE [dbo].[UsersHistoricalTransactions] (
    [Id]                              INT            IDENTITY (1, 1) NOT NULL,
    [UserId]                          NVARCHAR (450) NOT NULL,
    [CompetitionId]                   INT            NOT NULL,
    [StockId]                         INT            NOT NULL,
    [Quantity]                        INT            NOT NULL,
    [TransactionPrice]                MONEY          NOT NULL,
    [DateTimeOfTransaction]           DATETIME       NOT NULL,
    [CurrentHoldingsAfterTransaction] INT            NOT NULL,
    [BuyOrSell] NVARCHAR(4) NOT NULL, 
    [AveragePriceForCurrentHoldings] MONEY NOT NULL, 
    [CurrentPurchaseAmountForHoldings] MONEY NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competition] ([Id]),
    FOREIGN KEY ([StockId]) REFERENCES [dbo].[StaticStockData] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

