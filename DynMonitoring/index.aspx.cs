using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynMonitoring
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string GetLogs(string date)
        {
            var file = Path.Combine(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd") + ".log");
            string line = "";
            var data = File.ReadLines(file);
            data.ToList().ForEach(e => line += e + "<br />");

            return line;
        }
    }
}