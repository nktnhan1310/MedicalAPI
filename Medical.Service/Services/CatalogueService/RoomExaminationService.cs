﻿using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class RoomExaminationService : CatalogueService<RoomExaminations, SearchHopitalExtension>, IRoomExaminationService
    {
        public RoomExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<string> GetExistItemMessage(RoomExaminations item)
        {
            return await Task.Run(() =>
            {
                string result = string.Empty;
                if (Queryable.Any(x => !x.Deleted && x.Id != item.Id && x.HospitalId == item.Id && x.Code == item.Code))
                    return "Mã đã tồn tại!";
                return result;
            });
        }

        protected override Expression<Func<RoomExaminations, bool>> GetExpression(SearchHopitalExtension baseSearch)
        {
            return e => !e.Deleted
            && (!baseSearch.HospitalId.HasValue || e.HospitalId == baseSearch.HospitalId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent)
                || e.Code.Contains(baseSearch.SearchContent)
                || e.Name.Contains(baseSearch.SearchContent)
                || e.Description.Contains(baseSearch.SearchContent)
                );
        }

    }
}
