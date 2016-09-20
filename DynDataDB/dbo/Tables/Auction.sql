CREATE TABLE [dbo].[Auction] (
    [AuctionID]       INT      IDENTITY (1, 1) NOT NULL,
    [BranchID]        INT      NULL,
    [AuctionDateTime] DATETIME NULL,
    [modify_date]     DATETIME CONSTRAINT [DF_Auction] DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([AuctionID] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Auction]
    ON [dbo].[Auction]([AuctionID] ASC);


GO
create trigger trg_Auction on Auction
after update
as
begin
   set nocount on;

   update t set modify_date=getdate()
   from  Auction t 
   inner join inserted i on t.AuctionID=i.AuctionID

end