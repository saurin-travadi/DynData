CREATE TABLE [dbo].[Auction] (
    [AuctionID]       INT      IDENTITY (1, 1) NOT NULL,
    [BranchID]        INT      NULL,
    [AuctionDateTime] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([AuctionID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Auction]
    ON [dbo].[Auction]([AuctionID] ASC);

