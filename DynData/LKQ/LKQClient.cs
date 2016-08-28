using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynData.DDRAuction;

namespace DynData.LKQ
{
    public class Branch
    {
        public string BranchCode { get; set; }
        public DateTimeOffset[] AuctionDate { get; set; }
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

            if (response.WasSuccessful && response.RequestedData != null)
            {
                //parse response, get a list of branch and update property for next method.
                BranchList = response.RequestedData[0].BranchCodes.ToList().ConvertAll(c => new Branch() { BranchCode = c });

                //and send it to DB. Merge this list with existing branch list
                try
                {
                    clsDB.funcExecuteSQL("", "");
                }
                catch (Exception ex)
                {
                    clsLog.LogError("GetBranchList - " + ex.Message);
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
                    branch.AuctionDate = response.RequestedData.ToList().ConvertAll(c => c.AuctionDate).ToArray();
                    //send branch.AuctionDate array to DB. Merge this list with existing auction date by branch code
                    try
                    {
                        clsDB.funcExecuteSQL("", "");
                    }
                    catch (Exception ex)
                    {
                        clsLog.LogError("GetAuctionDates - Error while saving auction dates for branch " + branch.BranchCode + " Error : " + ex.Message);
                    }
                }
                else
                {
                    clsLog.LogInfo("GetAuctionDates - didn't get auction dates for branch " + branch.BranchCode);
                }
            });
        }

        private void GetStockList()
        {
            BranchList.ForEach(branch =>
            {
                branch.AuctionDate.ToList().ForEach(date =>
                {
                    var request = new GetStockListByAuctionDateByBranchRequest() { PartnerId = PartnerID, UserRequestInfo = User, BranchCode = branch.BranchCode, AuctionDate = date };
                    var response = Client.GetStockListByAuctionDateByBranch(request);

                    //Get response 

                    //Parse

                    //Save to DB

                });

                //System.Threading.Thread.Sleep(1000);
            });
        }
    }
}
