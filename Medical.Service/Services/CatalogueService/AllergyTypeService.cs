using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class AllergyTypeService : CatalogueService<AllergyTypes, BaseSearch>, IAllergyTypeService
    {
        public AllergyTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }
    }
}
