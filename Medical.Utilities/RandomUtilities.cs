using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Utilities
{
    public class RandomUtilities
    {
        private static Random random = new Random();
        /// <summary>
        /// Khởi tạo chuỗi random theo độ dài chuỗi
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
