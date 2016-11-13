using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DynData.DDRAuction;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using System.Text.RegularExpressions;

namespace DynData.LKQ
{
    public class LKQImageClient
    {
        public string GetImage(string StockNumber,int Index)
        {
            
            var image = new DeepZoom.Image();

            return image.Generate(StockNumber,Index);
        }

        public void GetData()
        {
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            DataTable dt = new DataTable();

            DataTable ImageDT = new DataTable();
            ImageDT.Columns.Add("StockNumber", typeof(string));
            ImageDT.Columns.Add("ImageIndex", typeof(int));
            ImageDT.Columns.Add("ImageUrl", typeof(string));
            
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_GETSTOCKNUMBER", con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        da.Fill(dt);
                    }

                }
            }

            for (int i =0; i<dt.Rows.Count;i++)
            {
                var StockNumber = dt.Rows[i]["StockNumber"].ToString();
                for(int j= 1; j <=10; j++)
                {
                    var ImageUrl = GetImage(StockNumber, j);

                    DataRow Row = ImageDT.NewRow();
                    Row["StockNumber"] = StockNumber;
                    Row["ImageIndex"] = j;
                    Row["ImageUrl"] = ImageUrl;
                    ImageDT.Rows.Add(Row);
                }
            }

            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    sqlBulkCopy.DestinationTableName = "dyndata.dbo.HighResStockImages";

                    sqlBulkCopy.ColumnMappings.Add("StockNumber", "StockNumber");
                    sqlBulkCopy.ColumnMappings.Add("ImageIndex", "ImageIndex");
                    sqlBulkCopy.ColumnMappings.Add("ImageUrl", "ImageUrl");

                    con.Open();
                    sqlBulkCopy.BulkCopyTimeout = 0;
                    sqlBulkCopy.WriteToServer(ImageDT);
                    con.Close();
                }
            }


        }
    }
}
