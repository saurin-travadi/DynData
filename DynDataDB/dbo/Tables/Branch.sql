CREATE TABLE [dbo].[Branch] (
    [BranchID]    INT      NOT NULL,
    [modify_date] DATETIME CONSTRAINT [DF_Branch] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED ([BranchID] ASC)
);




GO
create trigger trg_Branch on Branch
after update
as
begin
   set nocount on;

   update t set modify_date=getdate()
   from  Branch t 
   inner join inserted i on t.BranchID=i.BranchID

end