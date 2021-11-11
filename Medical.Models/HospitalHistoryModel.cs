using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Medical.Models
{
    public class HospitalHistoryModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Dữ liệu parse mới
        /// </summary>
        public string OldHospitalDataJson { get; set; }

        /// <summary>
        /// Dữ liệu parse mới
        /// </summary>
        public string NewHospitalDataJson { get; set; }

        #region Extension Properties

        /// <summary>
        /// Data bệnh viện cũ
        /// </summary>
        public HospitalModel HospitalOldData { get; set; }

        /// <summary>
        /// Data bệnh viện mới
        /// </summary>
        public HospitalModel HospitalNewData { get; set; }

        #endregion

    }
}
