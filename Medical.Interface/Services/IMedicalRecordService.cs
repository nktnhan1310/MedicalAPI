using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IMedicalRecordService : IDomainService<MedicalRecords, SearchMedicalRecord>
    {
        Task<int> GetMedicalRecordIdByUser(int userId);
    }
}
