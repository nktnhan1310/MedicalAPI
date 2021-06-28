using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class MedicalBillModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Số thứ tự chờ lấy thuốc khám bệnh
        /// </summary>
        public string MedicalBillIndex { get; set; }
        
        /// <summary>
        /// Tổng giá thuốc
        /// </summary>
        public double? TotalPrice { get; set; }
        
        /// <summary>
        /// Mã hồ sơ chi tiết bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }
        /// <summary>
        /// Trạng thái toa thuốc
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng thanh toán (nếu có)
        /// </summary>
        public int? BankInfoId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên ngân hàng thanh toán
        /// </summary>
        public string BankInfo { get; set; }

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case (int)CatalogueUtilities.MedicalBillStatus.New:
                        return "Mới";
                    case (int)CatalogueUtilities.MedicalBillStatus.Wait:
                        return "Chờ lấy thuốc";
                    case (int)CatalogueUtilities.MedicalBillStatus.Finished:
                        return "Hoàn thành đơn thuốc";
                    default:
                        break;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public string MedicalRecordCode { get; set; }

        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public string ExaminationFormCode { get; set; }

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string DoctorFullName { get; set; }

        /// <summary>
        /// Danh sách các loại thuốc
        /// </summary>
        public IList<MedicineModel> Medicines { get; set; }

        #endregion

    }
}
