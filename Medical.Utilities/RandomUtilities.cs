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

        /// <summary>
        /// Random chuỗi theo số + kí tự
        /// </summary>
        /// <param name="length"></param>
        /// <param name="lengthWord"></param>
        /// <param name="lengthNumber"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static string RandomString(int length, int? lengthWord, int? lengthNumber, bool isUpper = false)
        {
            string lowerWord = "abcdefghijklmnopqrstuvwxyz";
            string upperWord = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numberWord = "0123456789";
            string chars = string.Empty;
            string wordRandomString = string.Empty;
            string numberRandomString = string.Empty;
            string wordTypeRandomString = string.Empty;
            // Tạo chữ kiểu in hoa
            if (isUpper)
            {
                wordTypeRandomString = upperWord;
                if (lengthWord.HasValue && lengthWord.Value > 0)
                {
                    wordRandomString = new string(Enumerable.Repeat(upperWord, lengthWord.Value)
              .Select(s => s[random.Next(s.Length)]).ToArray());
                }

            }
            // Tạo chữ in thường
            else
            {
                wordTypeRandomString = lowerWord;
                if (lengthWord.HasValue && lengthWord.Value > 0)
                {
                    wordRandomString = new string(Enumerable.Repeat(lowerWord, lengthWord.Value)
              .Select(s => s[random.Next(s.Length)]).ToArray());
                }
            }

            // Khởi tạo chuỗi random số
            if (lengthNumber.HasValue && lengthNumber.Value > 0)
            {
                numberRandomString = new string(Enumerable.Repeat(numberWord, lengthNumber.Value)
          .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            // Tạo chuỗi random giữa số và chữ
            if (!string.IsNullOrEmpty(wordRandomString) && !string.IsNullOrEmpty(numberRandomString))
            {
                chars = wordRandomString + numberRandomString;
            }
            // Tạo chuỗi random chữ
            else if (!string.IsNullOrEmpty(wordRandomString) && length > lengthWord.Value)
            {
                int remainLength = length - lengthWord.Value;
                string remainNumberString = string.Empty;
                if (length > lengthWord.Value)
                    remainNumberString = new string(Enumerable.Repeat(numberWord, remainLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
                chars = wordRandomString + remainNumberString;
            }
            // Tạo chuỗi random số
            else if (!string.IsNullOrEmpty(numberRandomString))
            {
                int remainLength = length - lengthNumber.Value;
                string remainWordString = string.Empty;
                if (length > lengthNumber.Value)
                    remainWordString = new string(Enumerable.Repeat(wordTypeRandomString, remainLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());

                chars = numberRandomString + remainWordString;
            }
            // Tạo chuỗi random chữ thường + chữ hoa + số
            else
            {
                chars = new string(Enumerable.Repeat(lowerWord + upperWord + numberWord, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return chars;
        }

        /// <summary>
        /// Khởi tạo chuỗi OTP random từ 0-9
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomOTPString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
