CREATE PROCEDURE [dbo].[SP_PUSHDATA]
AS 

select ItemID, Lane, Slot, Start=SUBSTRING(Start,1,10), livedate, BranchCode, StockNo, VIN, VehicleYear, VehicleMake, VehicleModel, Transmission, 
            RunAndDrive, OdoBrand, Odometer, PrimaryDamage, SecondaryDamage, VehicleTitle, LossType, SaleDocument,
			ThumbnailURL
        from nonddrstock where 
		LiveDate = 'TBD' 
		and stockno not in (select stocknumber from stock)

UNION ALL

select ItemID, Lane, Slot, Start=SUBSTRING(Start,1,10), livedate=convert(varchar,convert(datetime,livedate),101)+' '+convert(varchar,convert(datetime,livedate),108), BranchCode, StockNo, VIN, VehicleYear, VehicleMake, VehicleModel, Transmission, 
			RunAndDrive, OdoBrand, Odometer, PrimaryDamage, SecondaryDamage, VehicleTitle, LossType, SaleDocument, ThumbnailURL
		from nonddrstock where 
		ISDATE(LiveDate)=1 
		and stockno not in (select stocknumber from stock)


ORDER BY StockNo