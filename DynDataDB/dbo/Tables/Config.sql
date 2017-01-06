CREATE TABLE [dbo].[Config] (
    [ID]    INT           IDENTITY (1, 1) NOT NULL,
    [key]   VARCHAR (50)  NULL,
    [value] VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

