using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Utilities
{
    public static class DateTimeUtilities
    {
        /// <summary>
        /// Parst chuỗi thời gian => tổng số phút
        /// </summary>
        /// <param name="timeText"></param>
        /// <returns></returns>
        public static int ConvertTimeToTotalIMinute(string timeText)
        {
            int timeResult = 0;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            if (TimeSpan.TryParse(timeText, out ts))
                timeResult = (int)ts.TotalMinutes;
            return timeResult;
        }

        /// <summary>
        /// Parse tổng số phút sang chuỗi giờ + phút + giây
        /// </summary>
        /// <param name="totalMinute"></param>
        /// <returns></returns>
        public static string ConvertTotalMinuteToString(int totalMinute)
        {
            string result = string.Empty;
            TimeSpan tsZero = new TimeSpan(0, 0, 0, 0);
            var currentDate = DateTime.Now.Date + tsZero;
            currentDate = currentDate.AddMinutes(totalMinute);
            result = currentDate.ToString("HH:mm:ss");
            return result;
        }

        /// <summary>
        /// Parse tổng số phút sang chuỗi giờ + phút
        /// </summary>
        /// <param name="totalMinute"></param>
        /// <returns></returns>
        public static string ConvertTotalMinuteToStringText(int totalMinute)
        {
            string result = string.Empty;
            TimeSpan tsZero = new TimeSpan(0, 0, 0, 0);
            var currentDate = DateTime.Now.Date + tsZero;
            currentDate = currentDate.AddMinutes(totalMinute);
            result = currentDate.ToString("HH:mm");
            return result;
        }

        public static DateTime? ToDate(this string date_s)
        {
            DateTime? resultDate = null;
            DateTime date = DateTime.Now;
            if (!string.IsNullOrEmpty(date_s))
            {
                if (DateTime.TryParseExact(date_s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "dd/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "d/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "d/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "dd/M/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
                else if (DateTime.TryParseExact(date_s, "d/M/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    resultDate = date;
                }
            }

            return resultDate;
        }

    }
}
