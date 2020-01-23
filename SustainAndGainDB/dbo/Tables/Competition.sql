CREATE TABLE [dbo].[Competition] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [StartTime] DATETIME       NOT NULL,
    [EndTime]   DATETIME       NOT NULL,
    [Name]      NVARCHAR (128) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

