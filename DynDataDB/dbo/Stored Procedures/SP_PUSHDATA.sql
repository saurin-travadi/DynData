CREATE PROCEDURE [dbo].[SP_PUSHDATA]
AS 


DECLARE @run DATETIME
SELECT @run = LastRun FROM dbo.Job where [JobName]='LKQPush'

select ItemID, Lane, Slot, Start=SUBSTRING(Start,1,10), livedate, BranchCode, StockNo, VIN, VehicleYear, VehicleMake, VehicleModel, Transmission, 
            RunAndDrive, OdoBrand, Odometer, PrimaryDamage, SecondaryDamage, VehicleTitle, LossType, SaleDocument,
			ThumbnailURL, modify_date
        from nonddrstock where 
		stockno not in (select stocknumber from stock)
		and modify_date>@run
ORDER BY StockNo