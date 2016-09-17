﻿using System;
using System.Collections.Generic;
using System.Linq;
using DynData.DDRAuction;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using System.Configuration;

namespace DynData.LKQ
{
    public class Branch
    {
        public string BranchCode { get; set; }
        public List<BranchAuction> Auctions { get; set; }
    }

    public class BranchAuction
    {
        public DateTime AuctionDate { get; set; }
        public List<BranchAuctionStock> StockNums { get; set; }
    }

    public class BranchAuctionStock
    {
        public string StockNum { get; set; }
    }

    public class FlatBranch
    {
        public string BranchCode { get; set; }
        public DateTime AuctionDate { get; set; }
        public string StockNum { get; set; }
    }

    public class LKQClient
    {
        public Guid VerificationCode { get; set; }
        public int PartnerID { get; set; }
        public AuctionClient Client { get; set; }

        private UserInformation User { get; set; }
        private List<Branch> BranchList { get; set; }



        public LKQClient()
        {
            PartnerID = 103;
            VerificationCode = new Guid("8e37acab-e44a-4fec-8509-3a469dd8ec08");
            User = new UserInformation() { VerificationCode = VerificationCode };

            Client = new AuctionClient();

        }

        public void GetData()
        {
            clsLog.LogInfo("Getting BranchList");
            GetBranchList();

            clsLog.LogInfo("Getting Auction Dates");
            GetAuctionDates();

            clsLog.LogInfo("Getting StockList");
            GetStockList();
        }

        public void PushData()
        {
            clsLog.LogInfo("Pushing Stocks to LKQ");

            try
            {
                var vehicleList = new List<VehicleInformationDto>();

                //Get a list of non DDR Stock from database
                //Create a stroed proc to return datatable, loop thru it and populate vehicleList. See sample below

                /*
               ItemID, Lane, Slot, Start, AuctionDate, BranchCode, StockNo, VIN, VehicleYear, VehicleMake, VehicleModel, Transmission, 
               RunAndDrive, OdoBrand, Odometer, PrimaryDamage, SecondaryDamage, VehicleTitle, LossType, SaleDocument, 

               ThumbnailURL, LargeURL => TALK TO SAURIN ON HOW TO GENERATE URL
               */

                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_PUSHDATA", con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;
                            da.Fill(dt);
                        }

                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var vehicle = new VehicleInformationDto()
                    {
                        ItemID = Convert.ToInt32(dt.Rows[i]["ItemID"]),
                        Lane = dt.Rows[i]["Lane"].ToString().Trim(),
                        Slot = dt.Rows[i]["Slot"].ToString().Trim(),
                        Start = dt.Rows[i]["Start"].ToString(),
                        BranchCode = Convert.ToInt32(dt.Rows[i]["BranchCode"]),
                        StockNo = dt.Rows[i]["StockNo"].ToString().Trim(),
                        VIN = dt.Rows[i]["VIN"].ToString().Trim(),
                        VehicleYear = dt.Rows[i]["VehicleYear"].ToString().Trim(),
                        VehicleMake = dt.Rows[i]["VehicleMake"].ToString().Trim(),
                        VehicleModel = dt.Rows[i]["VehicleModel"].ToString().Trim(),
                        Transmission = dt.Rows[i]["Transmission"].ToString().Trim(),
                        RunAndDrive = dt.Rows[i]["RunAndDrive"].ToString().Trim(),
                        OdoBrand = dt.Rows[i]["OdoBrand"].ToString().Trim(),
                        Odometer = dt.Rows[i]["Odometer"].ToString().Trim(),
                        PrimaryDamage = dt.Rows[i]["PrimaryDamage"].ToString().Trim(),
                        SecondaryDamage = dt.Rows[i]["SecondaryDamage"].ToString().Trim(),
                        VehicleTitle = dt.Rows[i]["VehicleTitle"].ToString().Trim(),
                        LossType = dt.Rows[i]["LossType"].ToString().Trim(),
                        SaleDocument = dt.Rows[i]["SaleDocument"].ToString().Trim()
                    };
                    if (!dt.Rows[i]["livedate"].ToString().ToLower().Equals("tbd"))
                        vehicle.AuctionDate = DateTimeOffset.Parse(dt.Rows[i]["livedate"].ToString());

                    //from database dt.Rows[i]["ThumbnailURL"].ToString() = https://vis.iaai.com/resizer?imageKeys=18200145~SID~B732~S1~I1~RW1280~H960~TH0&height=240&width=320
                    //replace vis.iaai.com with cvis.iaai.com
                    //replace ~RW1280~H960~TH0 with empty string

                    //Make Large URL 
                    //replace height=240 with height=480
                    //replace width=320 with width=640
                    //e.g. https://cvis.iaai.com/resizer?imageKeys=10419534~SID~B114~S0~I1&height=480&width=640

                    //Make Thumbnail URL 
                    //replace resizer with thumbnail
                    //replace height=240 with empty string
                    //replace width=320 with empty string
                    //e.g. https://cvis.iaai.com/thumbnail?imageKeys=10419534~SID~B114~S0~I1

                    //Now loop 1 to 10
                    //and replace ~I1 with ~I<index of loop>

                    vehicle.VehicleInformationImage = new VehicleInformationImage[10];
                    for (int image = 1; image <= 10; image++)
                    {
                        vehicle.VehicleInformationImage[image] = new VehicleInformationImage() { ThumbnailURL = "", LargeURL = "" };
                    }

                    vehicleList.Add(vehicle);
                }

                var request = new VehicleUploadRequest() { VehicleInformationList = vehicleList.ToArray(), UserRequestInfo = User };
                var response = Client.UploadVehicleInformation(request);
                if (!response.WasSuccessful)
                {
                    clsLog.LogInfo("PushData - didn't push data to remote");
                }
            }
            catch (Exception ex)
            {
                clsLog.LogInfo("PushData - Error ocurred while pushing data to remote. Error " + ex.Message);
            }
        }

        private void GetBranchList()
        {
            var request = new GetBranchListRequest() { PartnerIds = new int[] { PartnerID }, UserRequestInfo = User };
            var response = Client.GetBranchList(request);

            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            if (response.WasSuccessful && response.RequestedData != null)
            {
                try
                {
                    //parse response, get a list of branch and update property for next method.
                    BranchList = response.RequestedData[0].BranchCodes.ToList().ConvertAll(c => new Branch() { BranchCode = c });



                    var branchDataTable = new DataTable();
                    using (var reader = ObjectReader.Create(BranchList, "BranchCode"))
                    {
                        branchDataTable.Load(reader);
                    }


                    //and send it to DB. Merge this list with existing branch list
                    using (SqlConnection con = new SqlConnection(connection))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            sqlBulkCopy.DestinationTableName = "dyndata.dbo.branch_stg";
                            con.Open();
                            sqlBulkCopy.BulkCopyTimeout = 0;
                            sqlBulkCopy.WriteToServer(branchDataTable);

                            SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_BRANCH", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();

                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    clsLog.LogInfo("GetBranchList - Error while getting branch list from remote. Error " + ex.Message);
                }

            }
            else
            {
                clsLog.LogInfo("GetBranchList - didn't get branch list from remote");
            }
        }

        private void GetAuctionDates()
        {
            BranchList.ForEach(branch =>
            {
                var request = new GetBranchAuctionsRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = branch.BranchCode };
                var response = Client.GetAuctionDatesByBranch(request);

                if (response.WasSuccessful && response.RequestedData != null)
                {
                    branch.Auctions = new List<BranchAuction>(response.RequestedData.ToList().ConvertAll(c => new BranchAuction() { AuctionDate = c.AuctionDate.DateTime }).ToList());

                }
                else
                {
                    clsLog.LogInfo("GetAuctionDates - didn't get auction dates for branch " + branch.BranchCode);
                }
            });

            try
            {
                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

                var branchDataTable = new DataTable();

                var flatBranchList = from b in BranchList
                                     from a in b.Auctions

                                     select new FlatBranch { BranchCode = b.BranchCode, AuctionDate = a.AuctionDate };

                using (var reader = ObjectReader.Create(flatBranchList, "BranchCode", "AuctionDate"))
                {
                    branchDataTable.Load(reader);
                }

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dyndata.dbo.Auction_stg";
                        sqlBulkCopy.ColumnMappings.Add("BranchCode", "BranchID");
                        sqlBulkCopy.ColumnMappings.Add("AuctionDate", "AuctionDateTime");
                        con.Open();
                        sqlBulkCopy.BulkCopyTimeout = 0;
                        sqlBulkCopy.WriteToServer(branchDataTable);



                        SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_AUCTION", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLog.LogInfo("GetAuctionDates - Error while getting auction dates. Error " + ex.Message);
            }



        }

        private void GetStockList()
        {
            BranchList.ForEach(branch =>
            {
                branch.Auctions.ForEach(auction =>
                {
                    var request = new GetStockListByAuctionDateByBranchRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = branch.BranchCode, AuctionDate = auction.AuctionDate };
                    var response = Client.GetStockListByAuctionDateByBranch(request);

                    //Get response 
                    if (response.WasSuccessful && response.RequestedData != null)
                    {
                        auction.StockNums = new List<BranchAuctionStock>(response.RequestedData.ToList().ConvertAll(c => new BranchAuctionStock() { StockNum = c }).ToList());
                    }
                    else
                    {
                        clsLog.LogInfo("GetStockList - didn't get stock list for branch " + branch.BranchCode + " and auction date " + auction.AuctionDate.ToShortDateString());
                    }
                });

                System.Threading.Thread.Sleep(1000);
            });


            try
            {
                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                //and send it to DB. Merge this list with existing branch list

                var branchDataTable = new DataTable();

                var flatBranchList = from b in BranchList
                                     from a in b.Auctions
                                     from s in a.StockNums
                                     select new FlatBranch { BranchCode = b.BranchCode, AuctionDate = a.AuctionDate, StockNum = s.StockNum };

                using (var reader = ObjectReader.Create(flatBranchList))
                {
                    branchDataTable.Load(reader);
                }

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dyndata.dbo.Stock_stg";
                        sqlBulkCopy.ColumnMappings.Add("BranchCode", "BranchID");
                        sqlBulkCopy.ColumnMappings.Add("AuctionDate", "AuctionDateTime");
                        sqlBulkCopy.ColumnMappings.Add("StockNum", "StockNumber");
                        con.Open();
                        sqlBulkCopy.BulkCopyTimeout = 0;
                        sqlBulkCopy.WriteToServer(branchDataTable);



                        SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_STOCK", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLog.LogInfo("GetStockList - Error while getting stock list. Error " + ex.Message);
            }
        }
    }
}
