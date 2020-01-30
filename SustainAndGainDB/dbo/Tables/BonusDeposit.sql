CREATE TABLE [dbo].[BonusDeposit]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [UserId] NVARCHAR(450) NOT NULL, 
    [CompetitionId] INT NOT NULL, 
    [Bonus] MONEY NULL
    FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competition] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
)
