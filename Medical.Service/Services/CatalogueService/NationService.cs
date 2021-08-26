using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Medical.Service
{
    public class NationService : CatalogueService<Nations, BaseSearch>, INationService
    {
        public NationService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
            this.configuration = configuration;

        }
        protected override string GetTableName()
        {
            return "Nations";
        }

        protected override DataTable AddDataTableRow(DataTable dt, Nations item)
        {
            dt.Rows.Add(item.Id, item.CountryId, item.Created, item.CreatedBy, item.Updated, item.UpdatedBy, item.Deleted, item.Active, item.Code, item.Name, item.Description);
            return dt;
        }
    }
}
