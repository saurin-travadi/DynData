CREATE PROCEDURE [dbo].[SP_UpdatePushedStatus](
@stocknumbers varchar(max),
@status varchar(2000)='1'
)
AS
begin

create table #stock(Stock varchar(8),status int)
insert into #stock(Stock,status)
select Stock=a.items,Status=b.items from 
(
	select id,items from dbo.fn_Split(@stocknumbers,',')
) a
join
(
	select id,items from dbo.fn_Split(@status,',')
) b 
on a.id=b.id


update h 
set StockStatusCode = s.status
from HighResStock h join #stock s on h.StockNumber=s.Stock


drop table #stock



--update HighResStock set StockStatusCode = @status
--where stocknumber in (select items from dbo.fn_Split(@stocknumbers,','))

--update h
--set StockStatusCode=3
--from HighResStock h 
--join HighResStockImages i on h.StockNumber=i.StockNumber
--where HandlerName is not null and StockStatusCode is null
--and ImageUrl in ('Offsite','Specialty')

--update h
--set StockStatusCode=2
--from HighResStock h 
--join HighResStockImages i on h.StockNumber=i.StockNumber
--where HandlerName is not null and StockStatusCode is null
--and (ImageUrl is null or ImageUrl='')


end