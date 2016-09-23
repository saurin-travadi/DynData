using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynMonitoring
{
    public partial class update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LKQPush_Click(object sender, EventArgs e)
        {
            clsDB.funcExecuteSQL("SP_ReRunJob @job='" + ((Button)sender).Text + "'", ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
        }

        protected void IAAGet_Click(object sender, EventArgs e)
        {
            clsDB.funcExecuteSQL("SP_ReRunJob @job='" + ((Button)sender).Text + "'", ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
        }

        protected void LKQGet_Click(object sender, EventArgs e)
        {
            clsDB.funcExecuteSQL("SP_ReRunJob @job='" + ((Button)sender).Text + "'", ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
        }
    }
}