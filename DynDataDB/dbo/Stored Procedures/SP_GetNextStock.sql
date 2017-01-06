CREATE PROCEDURE [dbo].[SP_GetNextStock](
@Usrname varchar(20),
@stockNumber varchar(8) OUTPUT
)
AS

select @stockNumber = min(stocknumber)
        from highresstock where 
		ImageReviewed is null and handlername is not null
		and (lower(username) = lower(@Usrname) or UserName is null)