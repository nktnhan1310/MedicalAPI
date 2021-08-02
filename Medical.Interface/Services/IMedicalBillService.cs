using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IMedicalBillService : ICatalogueHospitalService<MedicalBills, SearchMedicalBill>
    {
        Task<bool> UpdateMedicalBillStatus(UpdateMedicalBillStatus updateMedicalBillStatus);

        Task<string> GetCheckStatusMessage(int medicalBillId, int statusCheck);
    }
}
