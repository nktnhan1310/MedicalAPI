using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardSaleResponse
    {
        /// <summary>
        /// Giá trị tháng
        /// </summary>
        public int? MonthValue { get; set; }
        /// <summary>
        /// Giá trị năm
        /// </summary>
        public int? YearValue { get; set; }
        /// <summary>
        /// Giá trị ngày thanh toán
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Ngày hiển thị
        /// </summary>
        public string PaymentDateDisplay
        {
            get
            {
                return PaymentDate.HasValue ? PaymentDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }


        /// <summary>
        /// Tổng chi phí thanh toán COD
        /// </summary>
        public double? TotalCODFee { get; set; }

        /// <summary>
        /// Tổng chi phí dịch vụ COD
        /// </summary>
        public double? TotalCODServiceFee { get; set; }

        /// <summary>
        /// Tổng chi phí thanh toán qua app
        /// </summary>
        public double? TotalAppFee { get; set; }

        /// <summary>
        /// Tổng chi phí dịch vụ qua app
        /// </summary>
        public double? TotalAppServiceFee { get; set; }

        /// <summary>
        /// TỔng thanh toán qua app
        /// </summary>
        public double? TotalPaymentApp
        {
            get
            {
                return (TotalAppFee ?? 0) + (TotalAppServiceFee ?? 0);
            }
        }
        /// <summary>
        /// Tổng thanh toán trực tiếp
        /// </summary>
        public double? TotalPaymentCOD
        {
            get
            {
                return (TotalCODFee ?? 0) + (TotalCODServiceFee ?? 0);
            }
        }

        /// <summary>
        /// Tổng giá trị thanh toán
        /// </summary>
        public double? TotalPayment
        {
            get
            {
                return (TotalPaymentApp ?? 0) + (TotalPaymentCOD ?? 0);
            }
        }

    }

    public class DashBoardSaleByHospitalResponse
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// Tổng chi phí thanh toán COD
        /// </summary>
        public double? TotalCODFee { get; set; }

        /// <summary>
        /// Tổng chi phí dịch vụ COD
        /// </summary>
        public double? TotalCODServiceFee { get; set; }

        /// <summary>
        /// Tổng chi phí thanh toán qua app
        /// </summary>
        public double? TotalAppFee { get; set; }

        /// <summary>
        /// Tổng chi phí dịch vụ qua app
        /// </summary>
        public double? TotalAppServiceFee { get; set; }

        /// <summary>
        /// TỔng thanh toán qua app
        /// </summary>
        public double? TotalPaymentApp
        {
            get
            {
                return (TotalAppFee ?? 0) + (TotalAppServiceFee ?? 0);
            }
        }
        /// <summary>
        /// Tổng thanh toán trực tiếp
        /// </summary>
        public double? TotalPaymentCOD
        {
            get
            {
                return (TotalCODFee ?? 0) + (TotalCODServiceFee ?? 0);
            }
        }
        /// <summary>
        /// Tổng thanh toán
        /// </summary>
        public double? TotalPayment
        {
            get
            {
                return (TotalPaymentApp ?? 0) + (TotalPaymentCOD ?? 0);
            }
        }
    }
}
