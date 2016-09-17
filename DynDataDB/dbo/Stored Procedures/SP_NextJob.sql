
CREATE PROCEDURE [dbo].[SP_NextJob]
@Job varchar(25)
as
BEGIN

	DECLARE @run DATETIME
	SELECT @run = NextRun FROM dbo.Job where [JobName]=@Job

	IF @run<getdate()
	BEGIN

		UPDATE dbo.Job SET LastRun=NextRun, NextRun=DATEADD(d,1,@run) where [JobName]=@Job

		SELECT 1

	END
	ELSE
		SELECT 0

END