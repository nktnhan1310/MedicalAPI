using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ReportExaminationFormModel : ReportAppDomainModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public string HospitalCode { get; set; }
        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// Tổng lịch hẹn mới
        /// </summary>
        public int TotalNewForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn chờ xác nhận
        /// </summary>
        public int TotalWaitConfirmForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn đã xác nhận
        /// </summary>
        public int TotalConfirmedForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn hủy
        /// </summary>
        public int TotalCanceledForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn chờ xác nhận tái khám
        /// </summary>
        public int TotalWaitReExaminationForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn đã xác nhận tái khám
        /// </summary>
        public int TotalConfirmedReExaminationForm { get; set; }

        public int TotalExamination
        {
            get
            {
                return TotalNewForm + TotalWaitConfirmForm + TotalConfirmedForm + TotalCanceledForm + TotalWaitReExaminationForm + TotalConfirmedReExaminationForm;
            }
        }
    }
}
