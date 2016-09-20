CREATE TABLE [dbo].[Stock] (
    [StockID]         INT      IDENTITY (1, 1) NOT NULL,
    [StockNumber]     INT      NULL,
    [BranchID]        INT      NULL,
    [AuctionDateTime] DATETIME NULL,
    [modify_date]     DATETIME CONSTRAINT [DF_Stock] DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([StockID] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Stock]
    ON [dbo].[Stock]([StockID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Stock_StockNumber]
    ON [dbo].[Stock]([StockNumber] ASC);


GO
create trigger trg_Stock on Stock
after update
as
begin
   set nocount on;

   update t set modify_date=getdate()
   from  Stock t 
   inner join inserted i on t.StockID=i.StockID 

end