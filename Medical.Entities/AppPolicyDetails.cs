using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class AppPolicyDetails : MedicalAppDomainHospital
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
