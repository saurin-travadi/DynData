CREATE PROCEDURE [dbo].[SP_GETSTOCKNUMBER] 
	@HandlerName VARCHAR(20)
AS 

BEGIN TRY
	DECLARE @StockList TABLE (ID int)

	INSERT INTO @StockList (ID)
	select TOP 10 ID from highresstock where 
			modify_date>'2016-12-12'
			and HandlerName is null
			order by ID

	if not exists(select 1 from @StockList)
	begin		--Retry download for Stocks tried earlier and didn't find images
		
		INSERT INTO @StockList (ID)
		select top 10 ID from (
			select distinct h.ID from HighResStock h join HighResStockImages i on h.StockNumber=i.StockNumber and StockStatusCode = 3
			where h.modify_date>'2016-12-12' 
			and ImageUrl not in ('offsite','specialty')
		) t
		order by ID

	end


	UPDATE [dbo].[HighResStock] SET [HandlerName]=@HandlerName, StockStatusCode=case when StockStatusCode=3 then -3 else StockStatusCode end
		WHERE ID IN (SELECT ID FROM @StockList)
			
	SELECT [StockNumber] FROM [dbo].[HighResStock] WHERE ID IN (SELECT ID FROM @StockList)


END TRY
BEGIN CATCH
	ROLLBACK 
END CATCH