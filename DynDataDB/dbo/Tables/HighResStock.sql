CREATE TABLE [dbo].[HighResStock] (
    [ID]              INT          IDENTITY (1, 1) NOT NULL,
    [StockNumber]     VARCHAR (8)  NULL,
    [modify_date]     DATE         DEFAULT (getdate()) NULL,
    [UserName]        VARCHAR (20) NULL,
    [HandlerName]     VARCHAR (20) NULL,
    [ImageReviewed]   INT          NULL,
    [StockStatusCode] INT          NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_HighResStock_StockNumber]
    ON [dbo].[HighResStock]([StockNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_HighResStock_StockStatusCode]
    ON [dbo].[HighResStock]([StockStatusCode] ASC)
    INCLUDE([StockNumber]);

