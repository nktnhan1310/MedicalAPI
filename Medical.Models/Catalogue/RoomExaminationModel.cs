using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Phòng khám bệnh
    /// </summary>
    public class RoomExaminationModel : MedicalCatalogueAppDomainModel
    {
        public int HospitalId { get; set; }
    }
}
