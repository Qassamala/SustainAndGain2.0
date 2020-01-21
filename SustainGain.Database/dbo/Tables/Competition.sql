CREATE TABLE [dbo].[Competition]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [IsOngoing] BIT NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NOT NULL
)
