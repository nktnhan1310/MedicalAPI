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
    public class SessionTypeService : CatalogueService<SessionTypes, BaseSearch>, ISessionTypeService
    {
        public SessionTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
