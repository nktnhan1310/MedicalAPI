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
    public class ConfigTimeExaminationService : CatalogueService<ConfigTimeExaminations, BaseSearch>, IConfigTimeExaminationService
    {
        public ConfigTimeExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
