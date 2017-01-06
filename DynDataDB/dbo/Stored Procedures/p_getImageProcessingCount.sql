CREATE procedure [dbo].[p_getImageProcessingCount]
as
begin


select 

'Total Stocks Processed'=(select [value] from [dbo].[Config] where [key] = 'HighResStockCount')+(select count(distinct StockNumber) from HighResStock h where modify_date>'2016-12-12')

, 'Images Pushed'=(select [value] from [dbo].[Config] where [key]= 'HighResStockImageCount')+(select 
	count(1) from HighResStock h join HighResStockImages i on h.StockNumber=i.StockNumber and StockStatusCode=1
	where h.modify_date>'2016-12-12' )

, 'Last Run DateTime' = dateadd(HOUR,-6,max(modify_date))

, 'Images Needed'=(select count(1) from HighResStock where StockStatusCode is null)

, 'Stocks in Q tobe pushed'=(select 
	count(distinct h.StockNumber) from HighResStock h join HighResStockImages i on h.StockNumber=i.StockNumber and StockStatusCode not in (1,2)
	where h.modify_date>'2016-12-12' and ImageUrl not in ('offsite','specialty'))

, 'No Images found for' = (select 
	count(distinct h.StockNumber) from HighResStock h where StockStatusCode in (2,3,4)
	and h.modify_date>'2016-12-12')+(select [value] from [dbo].[Config] where [key] = 'FailedStockImageCount')

from HighResStockImages 


end