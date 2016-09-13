using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynData.DDRAuction;
using System.Net;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DynData.IAA
{

    public class IAAClient
    {
        private WebClient Client { get; set; }
        private string BaseAddress { get; set; }
        private int CountPerCall { get; set; }

        public IAAClient()
        {
            Client = new WebClient();
            CountPerCall = 50;
            BaseAddress = "https://mapp.iaai.com/VehicleSearchService.svc/keyword/?devicetype=ios&count=" + CountPerCall + "&sortby=itemid&scope=all";
        }

        public void GetData()
        {
            GetStockList("AVIS", 1);
            GetStockList("ENTERPRISE", 1);
        }

        private void GetStockList(string search, int start)
        {
            try
            {
                var queryString = string.Format("&keyword={0}&start={1}", search, start);
                var response = Client.DownloadString(BaseAddress + queryString);

                // parse response using NewtonSoft, response is JSON and save to DB

                var bResponseEmpty = true;
                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

                DataTable dt = new DataTable();
                dt.Columns.Add("AuctionID", typeof(int));
                dt.Columns.Add("BranchCode", typeof(int));
                dt.Columns.Add("ItemID", typeof(int));
                dt.Columns.Add("Lane", typeof(string));
                dt.Columns.Add("LiveDate", typeof(string));
                dt.Columns.Add("LossType", typeof(string));
                dt.Columns.Add("OdoBrand", typeof(string));
                dt.Columns.Add("Odometer", typeof(string));
                dt.Columns.Add("PrimaryDamage", typeof(string));
                dt.Columns.Add("RunAndDrive", typeof(string));
                dt.Columns.Add("SaleDocument", typeof(string));
                dt.Columns.Add("SecondaryDamage", typeof(string));
                dt.Columns.Add("Slot", typeof(string));
                dt.Columns.Add("Start", typeof(string));
                dt.Columns.Add("StockNo", typeof(string));
                dt.Columns.Add("ThumbnailURL", typeof(string));
                dt.Columns.Add("Transmission", typeof(string));
                dt.Columns.Add("VehicleMake", typeof(string));
                dt.Columns.Add("VehicleModel", typeof(string));
                dt.Columns.Add("VehicleTitle", typeof(string));
                dt.Columns.Add("VIN", typeof(string));
                dt.Columns.Add("VehicleYear", typeof(string));

                dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                var actualData = x.actualData;
                foreach (var detail in actualData)
                {
                    DataRow Row = dt.NewRow();
                    Row["AuctionID"] = detail.auctionid;
                    Row["BranchCode"] = detail.branchcode;
                    Row["ItemID"] = detail.itemid;
                    Row["Lane"] = detail.lane;
                    Row["LiveDate"] = detail.livedate;
                    Row["LossType"] = detail.losstype;
                    Row["OdoBrand"] = detail.odobrand;
                    Row["Odometer"] = detail.odometer;
                    Row["PrimaryDamage"] = detail.primarydamage;
                    Row["RunAndDrive"] = detail.runanddrive;
                    Row["SaleDocument"] = detail.saledocument;
                    Row["SecondaryDamage"] = detail.secondarydamage;
                    Row["Slot"] = detail.slot;
                    Row["Start"] = detail.startcode;
                    Row["StockNo"] = detail.stockno;
                    Row["ThumbnailURL"] = detail.thumbnailurl;
                    Row["Transmission"] = detail.transmission;
                    Row["VehicleMake"] = detail.vehiclemake;
                    Row["VehicleModel"] = detail.vehiclemodel;
                    Row["VehicleTitle"] = detail.vehicletitle;
                    Row["VIN"] = detail.vin;
                    Row["VehicleYear"] = detail.year;
                    dt.Rows.Add(Row);
                    bResponseEmpty = false;
                }


                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dyndata.dbo.NonDDRStock";

                        sqlBulkCopy.ColumnMappings.Add("AuctionID", "AuctionID");
                        sqlBulkCopy.ColumnMappings.Add("BranchCode", "BranchCode");
                        sqlBulkCopy.ColumnMappings.Add("ItemID", "ItemID");
                        sqlBulkCopy.ColumnMappings.Add("Lane", "Lane");
                        sqlBulkCopy.ColumnMappings.Add("LiveDate", "LiveDate");
                        sqlBulkCopy.ColumnMappings.Add("LossType", "LossType");
                        sqlBulkCopy.ColumnMappings.Add("OdoBrand", "OdoBrand");
                        sqlBulkCopy.ColumnMappings.Add("Odometer", "Odometer");
                        sqlBulkCopy.ColumnMappings.Add("PrimaryDamage", "PrimaryDamage");
                        sqlBulkCopy.ColumnMappings.Add("RunAndDrive", "RunAndDrive");
                        sqlBulkCopy.ColumnMappings.Add("SaleDocument", "SaleDocument");
                        sqlBulkCopy.ColumnMappings.Add("SecondaryDamage", "SecondaryDamage");
                        sqlBulkCopy.ColumnMappings.Add("Slot", "Slot");
                        sqlBulkCopy.ColumnMappings.Add("Start", "Start");
                        sqlBulkCopy.ColumnMappings.Add("StockNo", "StockNo");
                        sqlBulkCopy.ColumnMappings.Add("ThumbnailURL", "ThumbnailURL");
                        sqlBulkCopy.ColumnMappings.Add("Transmission", "Transmission");
                        sqlBulkCopy.ColumnMappings.Add("VehicleMake", "VehicleMake");
                        sqlBulkCopy.ColumnMappings.Add("VehicleModel", "VehicleModel");
                        sqlBulkCopy.ColumnMappings.Add("VehicleTitle", "VehicleTitle");
                        sqlBulkCopy.ColumnMappings.Add("VIN", "VIN");
                        sqlBulkCopy.ColumnMappings.Add("VehicleYear", "VehicleYear");

                        con.Open();
                        sqlBulkCopy.BulkCopyTimeout = 0;
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }

                //if response is not empty, increase start by CountPerCall and call the same method recursively
                if (!bResponseEmpty)
                {
                    System.Threading.Thread.Sleep(1000);
                    GetStockList(search, start + CountPerCall - 1);
                }
            }
            catch (Exception ex)
            {
                clsLog.LogError("GetStockList - Error while saving IAA information with search " + search + " start " + start + " Error : " + ex.Message);
            }

        }
    }
}
