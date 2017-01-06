CREATE PROCEDURE [dbo].[SP_PUSHIMAGEDATA]
AS 
BEGIN

DECLARE @StockNumber INT

select top 1 @StockNumber=h.stocknumber from highresstockimages i join highresstock h
on i.StockNumber=h.StockNumber
where
	(StockStatusCode is null or StockStatusCode not in (-1,1,2,4))
	--and ImageReviewed = 1 
	and ImageUrl like '%.jpg'
order by h.modify_date 

UPDATE HighResStock set StockStatusCode=-1 where StockNumber=@StockNumber		--Temp status to mark being processed

select i.stocknumber, imageindex, imageurl from highresstockimages i join highresstock h
on i.StockNumber=h.StockNumber
where h.StockNumber=@StockNumber
	

END