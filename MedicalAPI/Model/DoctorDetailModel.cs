using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Chi tiết chuyên khoa từng bác sĩ
    /// </summary>
    public class DoctorDetailModel: MedicalAppDomainModel
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int SpecialistTypeId { get; set; }
        /// <summary>
        /// Chi phí khám theo bác sĩ
        /// </summary>
        public double? Price { get; set; }
    }
}
