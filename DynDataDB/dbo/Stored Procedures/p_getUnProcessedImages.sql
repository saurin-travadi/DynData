
CREATE procedure [dbo].[p_getUnProcessedImages]
as

begin

select * from HighResStockStatusCode

select * from HighResStock h
join HighResStockImages i on h.StockNumber=i.StockNumber
where HandlerName is not null and StockStatusCode is null
and ImageUrl =''



select * from HighResStock h
join HighResStockImages i on h.StockNumber=i.StockNumber
where HandlerName is not null and StockStatusCode is null
and ImageUrl is null

--delete from HighResStockImages where id in (
--select id from (
--select *,rno=ROW_NUMBER() over (partition by i.stocknumber, i.imageindex order by i.modify_date desc) from HighResStockImages i
--) t 
--where rno>1
--)

end