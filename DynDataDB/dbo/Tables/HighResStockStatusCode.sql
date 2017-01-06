CREATE TABLE [dbo].[HighResStockStatusCode] (
    [code]   INT          IDENTITY (1, 1) NOT NULL,
    [status] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC)
);

