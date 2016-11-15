using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace DynMonitoring
{
    
    public partial class ImageVerification : System.Web.UI.Page
    {
   
        protected void Page_Load(object sender, EventArgs e)
        {
                
        }

        [WebMethod]
        public static  String GetStockNo(String Name)
        {
            var connection = ConfigurationManager.ConnectionStrings ["Connection"].ConnectionString;
            var StockNumber = string.Empty;            
            if (Name != null)
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetNextStock", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Usrname", Name);
                        SqlParameter stockNumber = new SqlParameter("@stockNumber", SqlDbType.Int);
                        stockNumber.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(stockNumber);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        StockNumber = cmd.Parameters["@stockNumber"].Value.ToString();                        
                    }
                }
            }
            return StockNumber;
        }

        [WebMethod]
        public static int UpdateStockNo(String stockNo,int flag)
        {
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateStockStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", flag);
                    cmd.Parameters.AddWithValue("@stocknumber", stockNo);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return flag;
        }

        [WebMethod]
        public static string[] GetImages(String stockNo)
        {
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            String[] imageUrl = new string[10];
            DataTable dt = new DataTable();
            dt.Columns.Add("ImageUrl", typeof(String));            
            using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetImages", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@stocknumber", stockNo);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        da.Fill(dt);
                        con.Close();                        
                    }
                }
            for (int i = 0; i < 10; i++)
            {
                imageUrl[i] = dt.Rows[i]["ImageUrl"].ToString();                
            }
            return imageUrl;
        }
    }
}