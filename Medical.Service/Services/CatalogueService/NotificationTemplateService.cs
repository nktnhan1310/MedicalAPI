using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Medical.Service
{
    public class NotificationTemplateService : CatalogueHospitalService<NotificationTemplates, BaseHospitalSearch>, INotificationTemplateService
    {
        public NotificationTemplateService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }

        protected override Expression<Func<NotificationTemplates, bool>> GetExpression(BaseHospitalSearch baseSearch)
        {
            return e => !e.Deleted
            && (!baseSearch.HospitalId.HasValue || e.HospitalId == baseSearch.HospitalId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent)
                || e.Code.Contains(baseSearch.SearchContent)
                || e.Name.Contains(baseSearch.SearchContent)
                || e.Description.Contains(baseSearch.SearchContent)
                || e.Title.Contains(baseSearch.SearchContent)
                || e.Content.Contains(baseSearch.SearchContent)

                );
        }

    }
}
