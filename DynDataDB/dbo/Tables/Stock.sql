CREATE TABLE [dbo].[Stock] (
    [StockID]         INT      IDENTITY (1, 1) NOT NULL,
    [StockNumber]     INT      NULL,
    [BranchID]        INT      NULL,
    [AuctionDateTime] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([StockID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Stock]
    ON [dbo].[Stock]([StockID] ASC);

