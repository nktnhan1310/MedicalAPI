using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IMedicalRecordDetailService : ICoreHospitalService<MedicalRecordDetails, SearchMedicalRecordDetail>
    {
        Task<bool> UpdateMedicalRecordDetailFileAsync(int medicalRecordDetailId, IList<MedicalRecordDetailFiles> medicalRecordDetailFiles);
    }
}
