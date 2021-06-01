using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardResponse
    {
        /// <summary>
        /// Số lịch khám theo ngày
        /// </summary>
        public int TotalExaminationFormByDate { get; set; }
        /// <summary>
        /// Số lịch khám theo tháng
        /// </summary>
        public int TotalExaminationFormByMonth { get; set; }
        /// <summary>
        /// Số lịch khám theo năm
        /// </summary>
        public int TotalExaminationFormByYear { get; set; }

        /// <summary>
        /// Tổng số user theo ngày
        /// </summary>
        public int TotalUserByDate { get; set; }
        /// <summary>
        /// Tổng số user theo tháng
        /// </summary>
        public int TotalUserByMonth { get; set; }
        /// <summary>
        /// Tổng số user theo năm
        /// </summary>
        public int TotalUserByYear { get; set; }

        /// <summary>
        /// Số user đang active
        /// </summary>
        public int TotalUserActive { get; set; }

        /// <summary>
        /// Tổng số tiền thanh toán qua app theo ngày
        /// </summary>
        public double? TotalAppPriceByDate { get; set; }

        public string TotalAppPriceByDateDisplay
        {
            get
            {
                return TotalAppPriceByDate.HasValue ? TotalAppPriceByDate.Value.ToString("#,###") : string.Empty;
            }
        }

        /// <summary>
        /// Tổng số tiền thanh toán qua app theo tháng
        /// </summary>
        public double? TotalAppPriceByMonth { get; set; }
        public string TotalAppPriceByMonthDisplay
        {
            get
            {
                return TotalAppPriceByMonth.HasValue ? TotalAppPriceByMonth.Value.ToString("#,###") : string.Empty;
            }
        }
        /// <summary>
        /// Tổng số tiền thanh toán qua app theo năm
        /// </summary>
        public double? TotalAppPriceByYear { get; set; }
        public string TotalAppPriceByYearDisplay
        {
            get
            {
                return TotalAppPriceByYear.HasValue ? TotalAppPriceByYear.Value.ToString("#,###") : string.Empty;
            }
        }

        /// <summary>
        /// Tổng tiền thanh toán trực tiếp theo ngày
        /// </summary>
        public double? TotalCODPriceByDate { get; set; }
        public string TotalCODPriceByDateDisplay
        {
            get
            {
                return TotalCODPriceByDate.HasValue ? TotalCODPriceByDate.Value.ToString("#,###") : string.Empty;
            }
        }
        /// <summary>
        /// Tổng tiền thanh toán trực tiếp theo tháng
        /// </summary>
        public double? TotalCODPriceByMonth { get; set; }
        public string TotalCODPriceByMonthDisplay
        {
            get
            {
                return TotalCODPriceByMonth.HasValue ? TotalCODPriceByMonth.Value.ToString("#,###") : string.Empty;
            }
        }
        /// <summary>
        /// Tổng tiền thanh toán trực tiếp theo năm
        /// </summary>
        public double? TotalCODPriceByYear { get; set; }
        public string TotalCODPriceByYearDisplay
        {
            get
            {
                return TotalCODPriceByYear.HasValue ? TotalCODPriceByYear.Value.ToString("#,###") : string.Empty;
            }
        }


    }
}
