using Medical.Utilities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateQrCodeFile();
            //ConvertToTimeInt();
            TestAsync();
            //FindMissingNumber();
        }

        public static void ConvertToTimeInt()
        {
            string fromTime = "07:00:00";
            string toTime = "10:00:00";
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            DateTime currentDate = DateTime.Now.Date + ts;
            DateTime fromDate = DateTime.Now.Date + ts;
            DateTime toDate = DateTime.Now.AddDays(2).Date + ts;

            double fromTimeSpan = TimeSpan.Parse(fromTime).TotalMinutes;
            double toTimeSpan = TimeSpan.Parse(toTime).TotalMinutes;

            fromDate = fromDate.AddMinutes(fromTimeSpan);
            toDate = toDate.AddMinutes(toTimeSpan);

            Console.WriteLine(string.Format("From time span {0}", fromTimeSpan));
            Console.WriteLine(string.Format("To time span {0}", toTimeSpan));

            Console.WriteLine(string.Format("From date {0}", fromDate.ToString("dd/MM/yyyy hh:mm:ss")));
            Console.WriteLine(string.Format("To date {0}", toDate.ToString("dd/MM/yyyy hh:mm:ss")));

        }

        public static async Task TestAsync()
        {
            var result1 = CountAsync1();
            //var result2 = CountAsync2();
            //await Task.Delay(1000);
            //Thread n = new Thread(() =>
            //{
            //    Thread.Sleep(10000);
            //    Console.WriteLine(string.Format("Result 2: {0}", 0));
            //});
            //n.Start();
            int resultTest1 = await result1;
            //int resultTest2 = result2.Result;
            Console.WriteLine(string.Format("Result 1 after: {0}, result 2 after: {1}", resultTest1, 0));

            return;
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

        public static void FindMissingNumber()
        {
            //array to find the missing number between 1 and 10
            // Simplicity, We will take number 1 to 10 i where Number 5 is missing in the sequence.
            int[] arr = { 1, 2, 3 };

            var missingNumber = Enumerable.Range(1, 3).Except(arr).FirstOrDefault();

            Console.WriteLine("missing number  : {0}", missingNumber);
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

        private static async Task<int> CountAsync1()
        {
            await Task.Delay(2000); // 5 second delay
            return 1;
        }

        private static async Task<int> CountAsync2()
        {
            return await Task.Run(async () =>
            {
                await Task.Delay(5000); // 10 second delay
                Console.WriteLine(string.Format("Result 2: {0}", 2));
                return 2;
            }).ConfigureAwait(false);

        }

        

    }
}
