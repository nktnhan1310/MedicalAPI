using AutoMapper;
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
    public class CountryService : CatalogueService<Countries, BaseSearch>, ICountryService
    {
        public CountryService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy tên bảng
        /// </summary>
        /// <returns></returns>
        protected override string GetTableName()
        {
            return "Countries";
        }
    }
}
