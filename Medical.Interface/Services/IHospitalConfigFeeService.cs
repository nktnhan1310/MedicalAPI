using Medical.Entities;
using Medical.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IHospitalConfigFeeService : IDomainService<HospitalConfigFees, SearchHospitalConfigFee>
    {
        Task<FeeCaculateExaminationResponse> GetFeeExamination(FeeCaculateExaminationRequest feeCaculateExaminationRequest);
    }
}
