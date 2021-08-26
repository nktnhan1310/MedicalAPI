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
    public class AdditionServiceType : CatalogueHospitalService<AdditionServices, BaseHospitalSearch>, IAdditionServiceType
    {
        
        public AdditionServiceType(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
            this.configuration = configuration;
        }

        protected override string GetTableName()
        {
            return "AdditionServices";
        }

        protected override DataTable AddDataTableRow(DataTable dt, AdditionServices item)
        {
            dt.Rows.Add(item.Id, null, item.Created, item.CreatedBy, item.Updated, item.UpdatedBy, item.Deleted, item.Active, item.HospitalId, item.Code, item.Name, item.Description);
            return dt;
        }

    }
}
