﻿using AutoMapper;
using Medical.Entities;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class PermissionService : CatalogueService<Permissions, BaseSearch>, IPermissionService
    {
        public PermissionService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }
    }
}
