using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynData.DDRAuction;
using System.Configuration;


namespace LKQCTest
{
    [TestClass]
    public class LKQTest
    {
        public Guid VerificationCode { get; set; }
        public int PartnerID { get; set; }
        public AuctionClient Client { get; set; }

        private UserInformation User { get; set; }


        public LKQTest()
        {

            VerificationCode = new Guid(ConfigurationManager.AppSettings["Key"]);
            User = new UserInformation() { VerificationCode = VerificationCode };
            PartnerID = Convert.ToInt16(ConfigurationManager.AppSettings["ParnerId"]);


            Client = new AuctionClient();

        }

        [TestMethod]
        public void GetBranchListTest()
        {

            var request = new GetBranchListRequest() { PartnerIds = new int[] { PartnerID }, UserRequestInfo = User };
            var branchList = Client.GetBranchList(request);

            Assert.IsTrue(branchList.WasSuccessful);
            
        }

        [TestMethod]
        public void GetAuctionDateTest()
        {

            var request = new GetBranchAuctionsRequest() { PartnerId = PartnerID , UserRequestInfo = User, BranchCode = "811" };
            var auctionDateList = Client.GetAuctionDatesByBranch(request);

            Assert.IsTrue(auctionDateList.WasSuccessful);
               
           
        }

        [TestMethod]
        public void GetStockNoTest()
        {

            var request = new GetStockListByAuctionDateByBranchRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = "811", AuctionDate = DateTimeOffset.Parse("2016-11-07 11:00:00.0")  };
            var stockNo = Client.GetStockListByAuctionDateByBranch(request);

            Assert.IsTrue(stockNo.WasSuccessful);
            
        }

        [TestMethod]
        public void GetBranchInfoTest()
         {

             var branchcodeByParter = new BranchCodesByPartner() { BranchCodes = new string[] { "811" } , PartnerId = PartnerID};
             var request = new GetBranchInfoRequest() { BranchCodesByPartner = new BranchCodesByPartner[] { branchcodeByParter } , UserRequestInfo = User };
             var stockNo = Client.GetBranchInfo(request) ;

            Assert.IsTrue(stockNo.WasSuccessful);
            
         }

        [TestMethod]
        public void GetBuyItNowStocksTest()
        {

            var request = new PartnerRequest() { PartnerId = PartnerID, UserRequestInfo = User };
            var stocks = Client.GetBuyItNowStocks(request);

            Assert.IsTrue(stocks.WasSuccessful);
            
        }

        [TestMethod]
        public void GetChangedStockListbyLastAccessTest()
        {

            var request = new PartnerRequest() { PartnerId = PartnerID , UserRequestInfo = User };
            var stocksLastAccess = Client.GetChangedStockListbyLastAccess(request);

            Assert.IsTrue(stocksLastAccess.WasSuccessful);
            
        }

        [TestMethod]
        public void GetInterchangeByVinTest()
        {

            
            var request = new VehicleInterchangeInfoRequest() { UserRequestInfo = User , Vin = "WBA8E9C5XGK646786" };
            var vin = Client.GetInterchangeByVin(request);

            Assert.IsTrue(vin.WasSuccessful);
            
        }

        [TestMethod]
        public void GetPartnerIdsTest()
        {


            var request = new UserRequest() { UserRequestInfo = User };
            var parterId = Client.GetPartnerIds(request);

            Assert.IsTrue(parterId.WasSuccessful);
            
        }

        [TestMethod]
        public void GetStockCurrentBidTest()
        {


            var request = new CurrentBidRequest() { UserRequestInfo = User, BranchCode = "811", PartnerId = PartnerID, StockNumber = "17917854" };
            var stockBid = Client.GetStockCurrentBid(request);

            Assert.IsTrue(stockBid.WasSuccessful);
            
        }

        [TestMethod]
        public void GetStockInfoTest()
        {

            var stockNoByBranchCode = new StockNumbersByBranchCode() { BranchCode = "811", StockNumbers= new string[] { "17917854" } };
            var request = new GetStockInfoRequest() { PartnerId = PartnerID, UserRequestInfo = User, StockNumbersByBranchCode = stockNoByBranchCode };
            var stockInfo = Client.GetStockInfo(request);

            Assert.IsTrue(stockInfo.WasSuccessful);
            
        }
        [TestMethod]
        public void GetStocksRecentBidsTest()
        {


            var request = new PartnerRequest() { PartnerId = PartnerID, UserRequestInfo = User };
            var stockRecentBid = Client.GetStocksRecentBids(request);

            Assert.IsTrue(stockRecentBid.WasSuccessful);
            
        }
        [TestMethod]
        public void GetUtcAuctionDateByBranchTest()
        {


            var request = new GetBranchAuctionsRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = "811" };
            var utcAuctionDate = Client.GetUtcAuctionDateByBranch(request);

            Assert.IsTrue(utcAuctionDate.WasSuccessful);
            
        }
        [TestMethod]
        public void GetChangedStockListByAuctionDateByBranchTest()
        {


            var request = new GetStockListByAuctionDateByBranchRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = "811", AuctionDate = DateTimeOffset.Parse("2016-11-07 11:00:00.0") };
            var changedStockList = Client.GetChangedStockListByAuctionDateByBranch(request);

            Assert.IsTrue(changedStockList.WasSuccessful);
                        
        }



    }


}
