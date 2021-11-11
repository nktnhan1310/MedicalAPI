using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Medical.Service
{
    public class ConfigRoomExaminationService : DomainService<ConfigRoomExaminations, SearchConfigRoomExamination>, IConfigRoomExaminationService
    {
        public ConfigRoomExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ConfigRoomExamination_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchConfigRoomExamination baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@RoomExaminationId", baseSearch.RoomExaminationId),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                //new SqlParameter("SearchContent", baseSearch.SearchContent),
            };
            return parameters;
        }

    }
}
