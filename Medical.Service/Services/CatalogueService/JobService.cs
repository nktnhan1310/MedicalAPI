﻿using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service.Services
{
    public class JobService : CatalogueService<Jobs, BaseSearch>, IJobService
    {
        public JobService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }

        protected override string GetTableName()
        {
            return "Jobs";
        }

    }
}
