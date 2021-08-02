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
    public class SMSEmailTemplateService : CatalogueHospitalService<SMSEmailTemplates, BaseHospitalSearch>, ISMSEmailTemplateService
    {
        public SMSEmailTemplateService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
