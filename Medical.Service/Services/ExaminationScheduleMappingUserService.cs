using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class ExaminationScheduleMappingUserService : CoreHospitalService<ExaminationScheduleMappingUsers, BaseHospitalSearch>, IExaminationScheduleMappingUserService
    {
        public ExaminationScheduleMappingUserService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
