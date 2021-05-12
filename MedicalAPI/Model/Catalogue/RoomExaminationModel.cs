using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Phòng khám bệnh
    /// </summary>
    public class RoomExaminationModel : MedicalCatalogueAppDomainModel
    {
        public int HospitalId { get; set; }
    }
}
