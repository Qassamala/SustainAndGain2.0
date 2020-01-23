CREATE TABLE [dbo].[HistDataStocks] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [StockID]      INT           NOT NULL,
    [DateTime]     DATETIME      NOT NULL,
    [CurrentPrice] MONEY         NULL,
    [Symbol]       NVARCHAR (64) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([StockID]) REFERENCES [dbo].[StaticStockData] ([Id])
);

