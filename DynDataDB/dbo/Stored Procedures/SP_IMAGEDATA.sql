CREATE PROCEDURE [dbo].[SP_IMAGEDATA]
AS 
BEGIN

set nocount on

DECLARE @run DATETIME
SELECT @run = LastRun FROM dbo.Job where [JobName]='LKQPush'

CREATE TABLE #stocks (StockNumber INT)

INSERT INTO #stocks(StockNumber)
SELECT StockNumber FROM STOCK where modify_date>'2016-12-12'
UNION 
SELECT StockNo FROM NonDDRStock where modify_date>'2016-12-12'

INSERT INTO HighResStock (StockNumber)  
SELECT DISTINCT StockNumber FROM (
	SELECT StockNumber FROM #stocks 
	except
	SELECT [StockNumber] FROM [dbo].[HighResStock]
) t

DROP TABLE #stocks

END