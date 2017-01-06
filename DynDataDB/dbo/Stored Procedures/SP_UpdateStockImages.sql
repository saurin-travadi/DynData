
CREATE PROCEDURE [dbo].[SP_UpdateStockImages](
	@xml xml,
	@stockstatus int=null
)
AS
begin

	IF @stockstatus is not null

		--Give up after trying 2nd time downloading images
		UPDATE dbo.HighResStock set StockStatusCode=case when StockStatusCode=3 then 4 else @stockstatus end
		WHERE StockNumber IN (
			select 
				items.item.value('StockNumber[1]', 'varchar(8)') StockNumber 
			from @xml.nodes('//root/stock') items(item)
		)

	ELSE
	begin
		
		INSERT INTO [dbo].[HighResStockImages]([StockNumber],[ImageIndex],[ImageUrl],[modify_date])
		SELECT StockNumber,ImageIndex,ImageURL,modify_date FROM (
			select 
				items.item.value('StockNumber[1]', 'varchar(8)') StockNumber, 
				items.item.value('ImageIndex[1]', 'varchar(2)') ImageIndex, 
				items.item.value( 'ImageUrl[1]', 'varchar(250)') ImageURL,
				getdate() modify_date
			from @xml.nodes('//root/stock') items(item)
		) t		
		WHERE ImageURL not in ('specialty','offsite')


	end
end