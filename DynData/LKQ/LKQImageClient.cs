using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DynData.LKQImage;
using System.Net;

namespace DynData.LKQ
{
    public class LKQImageClient
    {
        public Guid VerificationCode { get; set; }
        public int PartnerID { get; set; }
        public ImageClient ImageClient { get; set; }
        private UserInformation User { get; set; }

        public LKQImageClient()
        {
            VerificationCode = new Guid(ConfigurationManager.AppSettings["Key"]);
            User = new UserInformation() { VerificationCode = VerificationCode };
            PartnerID = Convert.ToInt16(ConfigurationManager.AppSettings["ParnerId"]);

            ImageClient = new ImageClient();
        }
        public string GetImage(string StockNumber, int Index)
        {
            var img = new DeepZoom.Image();
            img.AccessKey = "AKIAIUI7FO46USBPEZZA";
            img.SecretAccessKey = "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";
            img.AWSRegion = Amazon.RegionEndpoint.USEast2;

            return img.Generate(StockNumber, Index);
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
                        cmd.Parameters.AddWithValue("@HandlerName", Environment.MachineName);

                        da.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var StockNumber = dt.Rows[i]["StockNumber"].ToString();
                for (int j = 1; j <= 10; j++)
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

        public void PushData()
        {
            var url = String.Empty;
            var stocknumber = String.Empty;
            int imageindex = 0;
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_PUSHIMAGEDATA", con))
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
                url = dt.Rows[i]["ImageUrl"].ToString().Trim();
                imageindex = Convert.ToInt32(dt.Rows[i]["imageindex"]);
                stocknumber = dt.Rows[i]["stocknumber"].ToString().Trim();
                var imageinfo = new ImageInformationDto()
                {
                    StockNumber = stocknumber,
                    ImageName = stocknumber + '_' + imageindex,
                    ImageByteArray = new WebClient().DownloadData(url)
                };

                var request = new ImageUploadRequest() { UserRequestInfo = User, ImageInformation = imageinfo };//{ UserRequestInfo = User, ImageInformation = imageinfolist};
                var response = ImageClient.UploadImageInformation(request);
                if(response.WasSuccessful)
                {
                    using (SqlConnection con = new SqlConnection(connection))
                    {
                        using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_UpdatePushedStatus", con))
                        {
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@stocknumber", stocknumber);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();                                
                            }
                        }
                    }
                }
            }
        }
    }
}
