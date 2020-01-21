CREATE TABLE [dbo].[Competition]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [IsOngoing] BIT NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NOT NULL
)
