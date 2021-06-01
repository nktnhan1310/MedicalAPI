using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class ConfigTimeExaminationService : CatalogueHospitalService<ConfigTimeExaminations, BaseHospitalSearch>, IConfigTimeExaminationService
    {
        public ConfigTimeExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
