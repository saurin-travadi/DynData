
CREATE PROCEDURE [dbo].[SP_ReRunJob]
@Job varchar(25)
as
BEGIN

	DECLARE @run DATETIME
	SELECT @run = LastRun FROM dbo.Job where [JobName]=@Job


	UPDATE dbo.Job SET NextRun=@run where [JobName]=@Job

END