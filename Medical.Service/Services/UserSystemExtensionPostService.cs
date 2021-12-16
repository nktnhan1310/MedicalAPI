﻿using AutoMapper;
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
    public class UserSystemExtensionPostService : DomainService<UserSystemExtensionPosts, SearchUserSystemExtensionPost>, IUserSystemExtensionPostService
    {
        public UserSystemExtensionPostService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "UserSystemExtensionPost_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchUserSystemExtensionPost baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@TargetTypeId", baseSearch.TargetTypeId),
                new SqlParameter("@PostTypeId", baseSearch.PostTypeId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

    }
}
