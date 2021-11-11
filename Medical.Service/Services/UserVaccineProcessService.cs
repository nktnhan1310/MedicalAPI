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
    public class UserVaccineProcessService : DomainService<UserVaccineProcesses, SearchUserVaccineProcess>, IUserVaccineProcessService
    {
        public UserVaccineProcessService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "UserVaccineProcess_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchUserVaccineProcess baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@VaccineTypeId", baseSearch.VaccineTypeId),
                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@MedicalRecordId", baseSearch.MedicalRecordId),
                new SqlParameter("@TypeId", baseSearch.TypeId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@InjectDate", baseSearch.InjectDate),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }
    }
}
