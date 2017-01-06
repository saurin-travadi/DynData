CREATE TABLE [dbo].[HighResStockImages] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [StockNumber] VARCHAR (8)   NULL,
    [ImageIndex]  INT           NULL,
    [ImageUrl]    VARCHAR (255) NULL,
    [modify_date] DATETIME      CONSTRAINT [DF_DateOfModification] DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_HighResStockImages_StockNumber]
    ON [dbo].[HighResStockImages]([StockNumber] ASC);

