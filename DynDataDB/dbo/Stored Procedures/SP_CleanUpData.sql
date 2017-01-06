
CREATE procedure [dbo].[SP_CleanUpData]
as
begin

delete from [dbo].[Auction] where modify_date<DATEADD(MONTH,-2,getdate())
delete from [dbo].[Stock] where modify_date<DATEADD(MONTH,-2,getdate())
delete from [dbo].[NonDDRStock] where modify_date<DATEADD(MONTH,-2,getdate())

declare @HighResStockCount INT
select @HighResStockCount=count(*) from [dbo].[HighResStock] where StockStatusCode=1 and modify_date<DATEADD(MONTH,-2,getdate())
delete from [dbo].[HighResStock] where StockStatusCode=1 and modify_date<DATEADD(MONTH,-2,getdate())

declare @FailedStockCount INT
select @FailedStockCount=count(distinct h.StockNumber) from HighResStockImages i join HighResStock h on h.StockNumber=i.StockNumber where StockStatusCode in (2,3,4)
delete from h from HighResStockImages i join HighResStock h on h.StockNumber=i.StockNumber where StockStatusCode in (2,3,4)

declare @HighResStockImageCount INT
select @HighResStockImageCount=count(*) from [dbo].[HighResStockImages] i join [dbo].[HighResStock] h on h.StockNumber=i.StockNumber where h.StockStatusCode=1
delete from i from [dbo].[HighResStockImages] i join [dbo].[HighResStock] h on h.StockNumber=i.StockNumber where h.StockStatusCode=1

update [dbo].[Config] set [value]=convert(int,[value])+@HighResStockCount where [key]='HighResStockCount'

update [dbo].[Config] set [value]=convert(int,[value])+@HighResStockImageCount where [key]='HighResStockImageCount'

update [dbo].[Config] set [value]=convert(int,[value])+@FailedStockCount where [key]='FailedStockImageCount'


end