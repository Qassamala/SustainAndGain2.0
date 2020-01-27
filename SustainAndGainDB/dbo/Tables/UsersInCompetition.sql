CREATE TABLE [dbo].[UsersInCompetition] (
    [Id]                                INT            IDENTITY (1, 1)NOT NULL,
    [UserId]                            NVARCHAR (450) NOT NULL,
    [CompId]                            INT            NOT NULL,
    [CurrentValue]                      MONEY          NULL,
    [AvailableForInvestment]            MONEY          NULL,
    [LastUpdatedCurrentValue]           DATETIME       NULL,
    [LastUpdatedAvailableForInvestment] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CompId]) REFERENCES [dbo].[Competition] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

