using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynData.DDRAuction;
using System.Net;

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
            var queryString = string.Format("&keyword={0}&start={1}", search, start);
            var response = Client.DownloadString(BaseAddress + queryString);

            // parse response using NewtonSoft, response is JSON and save to DB
            try
            {
                clsDB.funcExecuteSQL("", "");
            }
            catch (Exception ex)
            {
                clsLog.LogError("GetStockList - Error while saving IAA information with search " + search + " start " + start + " Error : " + ex.Message);
            }

            //if response is not empty, increase start by CountPerCall and call the same method recursively
            var bResponseEmpty = true;
            if (!bResponseEmpty)
            {
                //System.Threading.Thread.Sleep(1000);

                GetStockList(search, start + CountPerCall - 1);
            }
        }
    }
}
