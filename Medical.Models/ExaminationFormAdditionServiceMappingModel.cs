using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ExaminationFormAdditionServiceMappingModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int ExaminationFormId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int AdditionServiceId { get; set; }

        /// <summary>
        /// Phí dịch vụ
        /// </summary>
        public double? Amount { get; set; }

        #region Extension Properties

        public string AdditionServiceName { get; set; }

        #endregion
    }
}
