using System;
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
            GetBranchList();
            GetAuctionDates();
            GetStockList();
        }

        private void GetBranchList()
        {
            var request = new GetBranchListRequest() { PartnerIds = new int[] { PartnerID }, UserRequestInfo = User };
            var response = Client.GetBranchList(request);

            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            if (response.WasSuccessful && response.RequestedData != null)
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
            }


            );

            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            
            var branchDataTable = new DataTable();
            
            var flatBranchList = from b in BranchList
                                 from a in b.Auctions
                                     
                                 select new FlatBranch { BranchCode = b.BranchCode, AuctionDate = a.AuctionDate }; 
                        
            using (var reader = ObjectReader.Create(flatBranchList,"BranchCode","AuctionDate"))
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

                   
                });

                //System.Threading.Thread.Sleep(1000);
            });

           
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            //and send it to DB. Merge this list with existing branch list
           
            var branchDataTable = new DataTable();
            
            var flatBranchList = from b in BranchList
                                 from a in b.Auctions
                                 from s in a.StockNums
                                 select new FlatBranch { BranchCode = b.BranchCode, AuctionDate = a.AuctionDate , StockNum = s.StockNum };
           
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
    }
}
