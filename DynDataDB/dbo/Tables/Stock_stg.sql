CREATE TABLE [dbo].[Stock_stg] (
    [StockID]         INT      IDENTITY (1, 1) NOT NULL,
    [StockNumber]     INT      NULL,
    [BranchID]        INT      NULL,
    [AuctionDateTime] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([StockID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Stock_stg]
    ON [dbo].[Stock_stg]([StockID] ASC);

