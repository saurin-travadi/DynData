
CREATE PROCEDURE [dbo].[SP_GETSETTINGS] 
	@key VARCHAR(50)
AS 

SELECT [value] from [dbo].[Config]
	where [key]=@key