using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Medical.Entities
{
    public class HospitalHistories : MedicalAppDomainHospital
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
        /// Dữ liệu bệnh viện cũ
        /// </summary>
        public Hospitals HospitalOldData
        {
            get
            {
                Hospitals hospitals = null;
                if (!string.IsNullOrEmpty(OldHospitalDataJson))
                    hospitals = JsonSerializer.Deserialize<Hospitals>(OldHospitalDataJson);
                return hospitals;
            }
        }

        /// <summary>
        /// Dữ liệu bệnh viện mới
        /// </summary>
        public Hospitals HospitalNewData
        {
            get
            {
                Hospitals hospitals = null;
                if (!string.IsNullOrEmpty(OldHospitalDataJson))
                    hospitals = JsonSerializer.Deserialize<Hospitals>(NewHospitalDataJson);
                return hospitals;
            }
        }

        #endregion

    }
}
