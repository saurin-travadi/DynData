CREATE TABLE [dbo].[NonDDRStock] (
    [StockID]         INT           IDENTITY (1, 1) NOT NULL,
    [AuctionID]       INT           NULL,
    [BranchCode]      INT           NULL,
    [ItemID]          INT           NULL,
    [Lane]            CHAR (1)      NULL,
    [LiveDate]        VARCHAR (25)  NULL,
    [LossType]        VARCHAR (50)  NULL,
    [OdoBrand]        VARCHAR (50)  NULL,
    [Odometer]        VARCHAR (25)  NULL,
    [PrimaryDamage]   VARCHAR (50)  NULL,
    [RunAndDrive]     VARCHAR (10)  NULL,
    [SaleDocument]    VARCHAR (50)  NULL,
    [SecondaryDamage] VARCHAR (50)  NULL,
    [Slot]            VARCHAR (4)   NULL,
    [Start]           VARCHAR (100) NULL,
    [StockNo]         VARCHAR (25)  NOT NULL,
    [ThumbnailURL]    VARCHAR (200) NULL,
    [Transmission]    VARCHAR (50)  NULL,
    [VehicleMake]     VARCHAR (50)  NULL,
    [VehicleModel]    VARCHAR (50)  NULL,
    [VehicleTitle]    VARCHAR (50)  NULL,
    [VIN]             VARCHAR (17)  NULL,
    [VehicleYear]     VARCHAR (4)   NULL,
    [modify_date]     DATETIME      CONSTRAINT [DF_NonDDRStock] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_NonDDRStock_] PRIMARY KEY CLUSTERED ([StockID] ASC, [StockNo] ASC)
);








GO
create trigger trg_NonDDRStock on NonDDRStock
after update
as
begin
   set nocount on;

   update t set modify_date=getdate()
   from  NonDDRStock t 
   inner join inserted i on t.StockID=i.StockID and t.StockNo=i.StockNo

end
GO
CREATE NONCLUSTERED INDEX [IDX_NonDDRStock_ModifyDate]
    ON [dbo].[NonDDRStock]([modify_date] ASC)
    INCLUDE([StockNo]);

