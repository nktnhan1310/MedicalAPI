﻿using AutoMapper;
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
    public class DegreeTypeService : CatalogueService<DegreeTypes, BaseSearch>, IDegreeTypeService
    {
        public DegreeTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
            this.configuration = configuration;
        }

        protected override string GetTableName()
        {
            return "DegreeTypes";
        }
    }
}
