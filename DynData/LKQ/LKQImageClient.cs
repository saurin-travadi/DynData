using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DynData.LKQImage;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

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

            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        }

        //run once a day
        public void SetHighResData()
        {
            try
            {
                clsLog.LogInfo("SetHighResData - Getting Stock numbers available for HighRes");

                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                clsDB.funcExecuteSQL("SP_IMAGEDATA", connection, 300);

                clsLog.LogInfo("SetHighResData - Stocks for HighRes set");

            }
            catch (Exception ex)
            {
                clsLog.LogError("SetHighResData - Error while setting HighRes stock. Error " + ex.Message);
            }
        }

        //run every 10 minutes
        public void GetImageData()
        {
            try
            {
                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["Image_File_Path"]);
                if (new DirectoryInfo(imagePath).GetFiles().Count() > 2000)
                {
                    clsLog.Log("GetImageData - Skipping Get since more than 5000 images found in directory to be pushed");
                    return;
                }

                clsLog.LogInfo("GetImageData - Getting stock numbers for downloading");

                var ds = clsDB.funcExecuteSQLDS("SP_GETSTOCKNUMBER '" + Environment.MachineName + "'", connection, 300);
                DataTable dt = ds.Tables[0];

                var stocks = dt.Rows.OfType<DataRow>().ToList().ConvertAll(c => new Stock() { StockNumber = c["StockNumber"].ToString() });
                var images = new ConcurrentStack<Stock>();

                clsLog.LogInfo("GetImageData - Got stock numbers for downloading");

                Parallel.ForEach(stocks, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, p =>
                //stocks.ForEach(p =>
                {
                    var imageUrls = GetImage(p.StockNumber);
                    for (int j = 0; j < 10; j++)
                    {
                        images.Push(new Stock() { StockNumber = p.StockNumber, ImageIndex = j, ImageURL = imageUrls[j] });
                    }

                    clsLog.LogInfo("GetImageData - Got Image URLs for Stock " + p.StockNumber);

                    DataSet imageDS = new DataSet("root");
                    DataTable imageDT = new DataTable("stock");
                    imageDT.Columns.Add("StockNumber", typeof(string));
                    imageDT.Columns.Add("ImageIndex", typeof(int));
                    imageDT.Columns.Add("ImageUrl", typeof(string));
                    imageDS.Tables.Add(imageDT);
                    images.ToList().ForEach(e =>
                    {
                        imageDT.Rows.Add(imageDT.NewRow());
                        imageDT.Rows[imageDT.Rows.Count - 1]["StockNumber"] = e.StockNumber;
                        imageDT.Rows[imageDT.Rows.Count - 1]["ImageIndex"] = e.ImageIndex;
                        imageDT.Rows[imageDT.Rows.Count - 1]["ImageUrl"] = e.ImageURL;
                    });
                    if (imageDT.Rows[0]["ImageUrl"].ToString() == "Specialty" || imageDT.Rows[0]["ImageUrl"].ToString() == "Offsite")
                        clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}',@stockstatus=3", imageDS.GetXml()), connection);
                    else if (imageDT.Rows[0]["ImageUrl"].ToString() == "")
                        clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}',@stockstatus=2", imageDS.GetXml()), connection);
                    else
                        clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}'", imageDS.GetXml()), connection);

                    clsLog.LogInfo("GetImageData - HighRes Image URLs saved");

                });
            }
            catch (Exception ex)
            {
                clsLog.LogError("GetImageData - Error while Getting/Saving HighRes image urls. Error " + ex.Message);
            }

        }

        //run every 10 minutes
        public bool PushImageData()
        {
            bool successWhilePushing = true;

            try
            {
                clsLog.LogInfo("PushImageData - Getting StockImage URLs to push");

                var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connection))
                using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_PUSHIMAGEDATA", con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        da.Fill(dt);
                    }
                }
                var stocks = dt.Rows.OfType<DataRow>().ToList().ConvertAll(c => new Stock() { ImageIndex = Convert.ToInt16(c["imageindex"]), ImageURL = c["ImageUrl"].ToString().Trim(), StockNumber = c["stocknumber"].ToString().Trim() });
                var stack = new ConcurrentStack<Stock>(stocks);

                clsLog.LogInfo("PushImageData - Got StockImage URLs to push");


                Parallel.ForEach(stack, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, p =>
                //stocks.ForEach(p =>
                {
                    clsLog.LogInfo("PushImageData - Sending Stock Image URLs " + p.StockNumber);

                    var imageinfo = new ImageInformationDto()
                    {
                        StockNumber = p.StockNumber,
                        FileName = p.StockNumber + "_" + p.ImageIndex,
                        ImageByteArray = GetImageBytes(p.ImageURL)
                    };

                    try
                    {
                        var request = new ImageUploadRequest() { UserRequestInfo = User, ImageInformation = imageinfo };
                        var response = ImageClient.UploadImageInformation(request);
                        if (response.WasSuccessful)
                        {
                            clsLog.LogInfo("PushImageData - Sent Stock Image URLs " + p.StockNumber + "_" + p.ImageIndex);
                            p.Pushed = true;

                            File.Delete(p.ImageURL);
                        }
                        else
                        {
                            clsLog.LogInfo("PushImageData - Unable to send Stock Image URLs " + p.StockNumber + "_" + p.ImageIndex);
                        }
                    }
                    catch (Exception ex1)
                    {
                        clsLog.LogError("PushImageData - Error while sending HighRes stock Image urls. Stock" + p.StockNumber + "_" + p.ImageIndex + " Error " + ex1.Message);
                        successWhilePushing = false;
                    }
                });

                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dyndata.dbo.SP_UpdatePushedStatus", con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@stocknumbers", string.Join(",", stack.Where(w => w.Pushed).Select(s => s.StockNumber).ToArray()));
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                clsLog.LogError("PushImageData - Error while sending HighRes stock Image urls. Error " + ex.Message);
            }

            return successWhilePushing;
        }


        public void ProcessImage()
        {
            var connection = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            DataSet imageDS = new DataSet("root");
            DataTable imageDT = new DataTable("stock");
            imageDT.Columns.Add("StockNumber", typeof(string));
            imageDT.Columns.Add("ImageIndex", typeof(int));
            imageDT.Columns.Add("ImageUrl", typeof(string));
            imageDS.Tables.Add(imageDT);

            Dictionary<string, int> push = new Dictionary<string, int>();

            try
            {

                clsLog.LogInfo("ProcessImage - Getting stock numbers for downloading");
                var ds = clsDB.funcExecuteSQLDS("SP_GETSTOCKNUMBER '" + Environment.MachineName + "'", connection, 300);
                DataTable dt = ds.Tables[0];
                clsLog.LogInfo("ProcessImage - Got stock numbers for downloading");

                dt.Rows.OfType<DataRow>().ToList().ForEach(p =>
                {
                    var imageUrls = GetImage(p["StockNumber"].ToString());
                    clsLog.LogInfo("ProcessImage - Got Image URLs for Stock " + p["StockNumber"].ToString());


                    //if (imageUrls[0] == "Specialty" || imageUrls[0] == "Offsite")
                    //    clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}',@stockstatus=3", imageDS.GetXml()), connection);
                    //else if (string.IsNullOrEmpty(imageUrls[0]))
                    //    clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}',@stockstatus=2", imageDS.GetXml()), connection);
                    //else
                    //{
                    //    imageDT.Rows.Clear();
                        for (int i = 0; i < 10; i++)
                        {
                            imageDT.Rows.Add(imageDT.NewRow());
                            imageDT.Rows[imageDT.Rows.Count - 1]["StockNumber"] = p["StockNumber"].ToString();
                            imageDT.Rows[imageDT.Rows.Count - 1]["ImageIndex"] = i;
                            imageDT.Rows[imageDT.Rows.Count - 1]["ImageUrl"] = imageUrls[i];
                        }
                        //clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}'", imageDS.GetXml()), connection);
                    //}
                    clsLog.LogInfo("ProcessImage - HighRes Image URLs saved");


                    clsLog.LogInfo("ProcessImage - Sending Stock Image URLs " + p["StockNumber"].ToString());

                    var pushedSuccessful = true;
                    if (imageUrls[0] == "Specialty" || imageUrls[0] == "Offsite")
                        push.Add(p["StockNumber"].ToString(), 3);
                    else if (string.IsNullOrEmpty(imageUrls[0]))
                        push.Add(p["StockNumber"].ToString(), 2);
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var imageinfo = new ImageInformationDto()
                            {
                                StockNumber = p["StockNumber"].ToString(),
                                FileName = p["StockNumber"].ToString() + "_" + i,
                                ImageByteArray = GetImageBytes(imageUrls[i])
                            };

                            clsLog.LogInfo("ProcessImage - Sending Stock Image URLs " + p["StockNumber"].ToString() + " Index " + i);
                            var request = new ImageUploadRequest() { UserRequestInfo = User, ImageInformation = imageinfo };
                            var response = ImageClient.UploadImageInformation(request);
                            if (response.WasSuccessful)
                            {
                                clsLog.LogInfo("ProcessImage - Sent Stock Image URLs " + p["StockNumber"] + "_" + i);

                                File.Delete(imageUrls[i]);
                            }
                            else
                            {
                                pushedSuccessful = false;
                                clsLog.LogError("ProcessImage - Unable to send Stock Image URLs " + p["StockNumber"] + "_" + i);
                            }
                        }

                        if(!push.Keys.ToList().Exists(e=>p["StockNumber"].ToString()==e))
                            push.Add(p["StockNumber"].ToString(), pushedSuccessful ? 1 : 5);

                    }

                    //clsDB.funcExecuteSQL(string.Format("SP_UpdatePushedStatus @stocknumbers='{0}', @status='{1}'", p["StockNumber"].ToString(), pushedSuccessful ? 1 : 5), connection);

                });



                //-------------------------------------
                clsDB.funcExecuteSQL(string.Format("SP_UpdateStockImages @xml='{0}'", imageDS.GetXml()), connection);
                clsDB.funcExecuteSQL(string.Format("SP_UpdatePushedStatus @stocknumbers='{0}', @status='{1}'", string.Join(",",push.Keys), string.Join(",",push.Values)), connection);


            }
            catch (Exception ex)
            {
                clsLog.LogError("ProcessImage - Error while executing. " + ex.Message);
            }
        }

        private string[] GetImage(string StockNumber)
        {
            var deepZomImage = new DeepZoom.Image();
            deepZomImage.WorkingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["Log_File_Path"]);
            deepZomImage.ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["Image_File_Path"]);

            var imgPath = deepZomImage.Generate(StockNumber);
            deepZomImage = null;

            return imgPath;
        }

        private byte[] GetImageBytes(string imagePath)
        {
            clsLog.LogInfo("GetImageBytes - Trying to get image array for " + imagePath);

            var img = System.Drawing.Image.FromFile(imagePath);
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                arr = ms.ToArray();
                ms.Close();
            }
            img.Dispose();

            clsLog.LogInfo("GetImageBytes - Returning image array for " + imagePath);

            return arr;
        }

        private class Stock
        {
            public string ImageURL { get; set; }
            public int ImageIndex { get; set; }
            public string StockNumber { get; set; }
            public bool Pushed { get; set; }
        }
    }
}
