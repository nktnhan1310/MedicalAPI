using BarcodeLib;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using static QRCoder.PayloadGenerator;

namespace Medical.Utilities
{
    public class QRCodeUtils
    {
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        public QRCodeUtils(IConfiguration configuration, IHttpContextAccessor _httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = _httpContextAccessor;
        }

        public string GetQrImagePath(int userId, int recordDetailId)
        {
            string fileQrCodeImgPath = string.Empty;
            bool isProduct = false;
            var productSecton = configuration.GetSection("MySettings:IsProduct");
            if (productSecton != null)
                bool.TryParse(productSecton.Value, out isProduct);
            string urlResult = string.Empty;
            string apiUrl = string.Format("api/medical-record-detail/get-record-detail-info-by-user/{0}/{1}", userId, recordDetailId);
            string appDomainUrl = string.Empty;
            var appDomainUrlSecton = configuration.GetSection("MySettings:AppDomainUrl");
            if (appDomainUrlSecton != null)
                appDomainUrl = appDomainUrlSecton.Value.ToString();
            urlResult = appDomainUrl + apiUrl;
            string fileName = Guid.NewGuid().ToString() + "_qrCode.png";
            var directorySection = configuration.GetSection("MySettings:FolderUpload");
            string folderPath = Path.Combine(directorySection.Value.ToString(), CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, CoreContants.QR_CODE_FOLDER_NAME);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            Url url = new Url(urlResult);
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            byte[] contentQrCodeImage = BitmapToBytes(qrCodeImage);
            FileUtils.CreateDirectory(folderPath);
            FileUtils.SaveToPath(Path.Combine(folderPath, fileName), contentQrCodeImage);
            fileQrCodeImgPath = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, CoreContants.QR_CODE_FOLDER_NAME, fileName);
            return fileQrCodeImgPath;
        }

        /// <summary>
        /// Khởi tạo barcode
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordDetailId"></param>
        /// <returns></returns>
        public string GetBarCodeImagePath(string medicalRecordCode)
        {
            string fileQrCodeImgPath = string.Empty;
            bool isProduct = false;
            var productSecton = configuration.GetSection("MySettings:IsProduct");
            if (productSecton != null)
                bool.TryParse(productSecton.Value, out isProduct);
            string appDomainUrl = string.Empty;
            var appDomainUrlSecton = configuration.GetSection("MySettings:AppDomainUrl");
            if (appDomainUrlSecton != null)
                appDomainUrl = appDomainUrlSecton.Value.ToString();
            string fileName = Guid.NewGuid().ToString() + "_barCode.png";
            var directorySection = configuration.GetSection("MySettings:FolderUpload");
            string folderPath = Path.Combine(directorySection.Value.ToString(), CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, CoreContants.BAR_CODE_FOLDER_NAME);

            // Tạo bar code
            Barcode barcode = new Barcode();
            Image img = barcode.Encode(TYPE.CODE128, medicalRecordCode, Color.Black, Color.White, 250, 100);
            //var bitMapBarCode = myBarCode.ToBitmap();
            byte[] contentBarCodeImage = ConvertImageToBytes(img);
            // Lưu file
            FileUtils.CreateDirectory(folderPath);
            FileUtils.SaveToPath(Path.Combine(folderPath, fileName), contentBarCodeImage);
            fileQrCodeImgPath = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, CoreContants.BAR_CODE_FOLDER_NAME, fileName);
            return fileQrCodeImgPath;
        }


        /// <summary>
        /// Save to Image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Save to image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private byte[] ConvertImageToBytes(Image img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

    }
}
