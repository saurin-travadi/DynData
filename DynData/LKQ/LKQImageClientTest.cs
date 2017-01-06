using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynData.LKQImage;
using System.Configuration;
using System.Collections.Generic;
using System.Net;

namespace LKQCTest
{
    [TestClass]
    public class LKQImageTest
    {
        public Guid VerificationCode { get; set; }
        public int PartnerID { get; set; }
        public ImageClient ImageClient { get; set; }
        private UserInformation User { get; set; }


        public LKQImageTest()
        {

            VerificationCode = new Guid(ConfigurationManager.AppSettings["Key"]);
            User = new UserInformation() { VerificationCode = VerificationCode };
            PartnerID = Convert.ToInt16(ConfigurationManager.AppSettings["ParnerId"]);


            ImageClient = new ImageClient();
        }

        [TestMethod]
        public void UploadImageinformationTest()
        {

            var vehicle = new ImageInformationDto();
            var large = "https://cvis.iaai.com/thumbnail?imageKeys=18693774~SID~I1";
            vehicle.StockNumber = "18693774";
            vehicle.FileName = "18693774_1";
            vehicle.ImageByteArray = new WebClient().DownloadData(large);

            var request = new ImageUploadRequest() { UserRequestInfo = User, ImageInformation = vehicle };
            var uploadStockImage = ImageClient.UploadImageInformation(request);

            Assert.IsTrue(uploadStockImage.WasSuccessful);

        }


    }


}
