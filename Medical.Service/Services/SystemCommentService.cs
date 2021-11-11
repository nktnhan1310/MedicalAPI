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
    public class SystemCommentService : DomainService<SystemComments, SearchSystemComment>, ISystemCommentService
    {
        public SystemCommentService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "SystemComment_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchSystemComment baseSearch)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@Email", baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@UserId", baseSearch.UserId),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return sqlParameters;
        }
    }
}
