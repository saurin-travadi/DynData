CREATE PROCEDURE [dbo].[SP_GetImages](

@stockNumber varchar(8) 
)
AS

select imageurl = case when ImageUrl like 'http%' then ImageUrl else replace(ImageUrl,'C:\DynData\service\','') end
        from highresstockimages where 
		stocknumber = @stockNumber;