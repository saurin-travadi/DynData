CREATE PROCEDURE [dbo].[SP_UpdateStockStatus](
@Flag int,
@stocknumber varchar(8)
)
AS

update HighResStock set ImageReviewed = @Flag 
where stocknumber = @stocknumber;