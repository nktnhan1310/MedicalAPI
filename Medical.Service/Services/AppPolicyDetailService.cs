using AutoMapper;
using Medical.Entities;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class AppPolicyDetailService : CoreHospitalService<AppPolicyDetails, BaseHospitalSearch>, IAppPolicyDetailService
    {
        public AppPolicyDetailService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }
    }
}
