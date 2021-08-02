using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ExaminationScheduleDetailService : DomainService<ExaminationScheduleDetails, BaseSearch>, IExaminationScheduleDetailService
    {
        public ExaminationScheduleDetailService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationScheduleDetailV2_GetInfo";
        }

        protected override SqlParameter[] GetSqlParameters(BaseSearch baseSearch)
        {
            return base.GetSqlParameters(baseSearch);
        }
    }
}
