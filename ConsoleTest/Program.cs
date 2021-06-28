using Medical.Utilities;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateQrCodeFile();
        }

        public static void CreateQrCodeFile()
        {
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //Bookmark generator = new Bookmark("https://www.taskade.com/d/xtYBX1qcFp2T1iyW?edit=Q9bYyKtq5xzHqaGY&share=edit", "Blog of QRCoder's father");
            //Url url = new Url("https://www.taskade.com/d/xtYBX1qcFp2T1iyW?edit=Q9bYyKtq5xzHqaGY&share=edit");
            //string payload = generator.ToString();

            //QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //byte[] contentQrCodeImage = BitmapToBytes(qrCodeImage);
            //string imageName = "testQr2.png";
            //string filePath = @"E:\\MonaMedia";
            //FileUtils.CreateDirectory(filePath);
            //FileUtils.SaveToPath(Path.Combine(filePath, imageName), contentQrCodeImage);
            //QRCodeUtils qRCodeUtils = new QRCodeUtils();
            //var filePath = GetQrImagePath(1, 1);
            //Console.WriteLine(filePath);
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
    }
}
