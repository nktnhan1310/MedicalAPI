using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IDashBoardService
    {
        /// <summary>
        /// Lấy tổng phiếu hẹn theo trạng thái
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        Task<DashBoardResponse> GetTotalExaminationByRequest(DashBoardRequest dashBoardRequest);

        /// <summary>
        /// Lấy tổng user của hệ thống
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        Task<DashBoardResponse> GetTotalUser(DashBoardRequest dashBoardRequest);

        /// <summary>
        /// Lấy thông tin user hệ thống
        /// </summary>
        /// <param name="searchDashBoardUserSystem"></param>
        /// <returns></returns>
        Task<double> GetTotalUserSystem(SearchDashBoardUserSystem searchDashBoardUserSystem);

        /// <summary>
        /// Lấy tổng số user khám bệnh ngày/tháng/năm
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        Task<DashBoardResponse> GetTotalUserExamination(DashBoardRequest dashBoardRequest);

        /// <summary>
        /// Lấy tổng số tiền thanh toán theo phương thức thanh toán
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        Task<DashBoardResponse> GetTotalPaymentSystem(DashBoardRequest dashBoardRequest);

        /// <summary>
        /// Lấy thông tin chi phí đã thanh toán/hoàn tiền cho hệ thống
        /// </summary>
        /// <param name="searchDashBoardTotalPayment"></param>
        /// <returns></returns>
        Task<double> GetTotalPaymentV2(SearchDashBoardTotalPayment searchDashBoardTotalPayment);

        /// <summary>
        /// Lấy thông tin danh sách báo cáo tổng hợp
        /// </summary>
        /// <param name="dashBoardSynthesisRequest"></param>
        /// <returns></returns>
        Task<List<DashBoardSynthesisResponse>> GetSynthesisReport(DashBoardSynthesisRequest dashBoardSynthesisRequest);

        /// <summary>
        /// Lấy danh sách bệnh viện
        /// </summary>
        /// <param name="dashBoardSynthesisRequest"></param>
        /// <returns></returns>
        Task<List<DashBoardSynthesisByHospitalResponse>> GetSynthesisReportByHospital(DashBoardSynthesisRequest dashBoardSynthesisRequest);

        /// <summary>
        /// Lấy số liệu báo cáo doanh thu theo ngày/tháng/năm
        /// </summary>
        /// <param name="dashBoardSaleRequest"></param>
        /// <returns></returns>
        Task<List<DashBoardSaleResponse>> GetSaleReport(DashBoardSaleRequest dashBoardSaleRequest);

        /// <summary>
        /// Lấy danh sách bệnh viện với doanh thu
        /// </summary>
        /// <param name="dashBoardSaleRequest"></param>
        /// <returns></returns>
        Task<List<DashBoardSaleByHospitalResponse>> GetSaleReportByHospital(DashBoardSaleRequest dashBoardSaleRequest);
    }
}
