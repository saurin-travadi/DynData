CREATE TABLE [dbo].[STOCK_STG_IMAGES] (
    [StockNO]    VARCHAR (25)  NULL,
    [ImageIndex] INT           IDENTITY (1, 1) NOT NULL,
    [ImageURL]   VARCHAR (200) NULL,
    [Indicator]  VARCHAR (4)   NULL
);

