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
    public class ExaminationFormAdditionServiceDetailMappingService : CoreHospitalService<ExaminationFormAdditionServiceDetailMappings, BaseHospitalSearch>, IExaminationFormAdditionServiceDetailMappingService
    {
        public ExaminationFormAdditionServiceDetailMappingService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }
    }
}
