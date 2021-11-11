using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class AppPolicyDetailModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Chính sách của app
        /// </summary>
        public int AppPolicyId { get; set; }

        /// <summary>
        /// Nội dung chính sách
        /// </summary>
        public string Content { get; set; }
    }
}
