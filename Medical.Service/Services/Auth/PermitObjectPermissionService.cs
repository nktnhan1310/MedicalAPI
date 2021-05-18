using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class PermitObjectPermissionService : DomainService<PermitObjectPermissions, BaseSearch>, IPermitObjectPermissionService
    {
        public PermitObjectPermissionService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
