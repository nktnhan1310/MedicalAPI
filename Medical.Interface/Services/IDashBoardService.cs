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

    }
}
