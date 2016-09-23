using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
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
            if (IsPostBack)
                return;

            var dir = ConfigurationManager.AppSettings["FilePath"];
            new DirectoryInfo(dir).GetFiles().ToList().OrderByDescending(o=>o.Name).Take(5).ToList().ForEach(f =>
            {
                form1.Controls.Add(new LinkButton() { Text = f.Name, OnClientClick = "return getLogs('" + f.Name + "')" });
            });
        }

        [WebMethod]
        public static string GetLogs(string date)
        {
            if (date == "")
                date = DateTime.Today.ToString("yyyyMMdd")+ ".log";
                

            var file = Path.Combine(ConfigurationManager.AppSettings["FilePath"], date);
            StringBuilder line = new StringBuilder("");
            if (!File.Exists(file))
                line.AppendLine("No logs available");
            else
            {
                var data = File.ReadLines(file);
                data.ToList().ForEach(e => line.Insert(0,e + "<br />"));
            }
            return line.ToString();
        }
    }
}