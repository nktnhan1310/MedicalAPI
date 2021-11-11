using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class HospitalHistoryService : CoreHospitalService<HospitalHistories, BaseHospitalSearch>, IHospitalHistoryService
    {
        public HospitalHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "HospitalHisotry_GetPagingData";
        }

    }
}
