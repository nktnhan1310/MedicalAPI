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
    public class ExaminationTypeService : CatalogueService<ExaminationTypes, BaseSearch>, IExaminationTypeService
    {
        public ExaminationTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
