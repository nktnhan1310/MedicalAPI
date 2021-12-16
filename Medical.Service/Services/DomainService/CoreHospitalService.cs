using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface.DbContext;
using Medical.Interface.Services.Base;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Medical.Service.Services.DomainService
{
    public abstract class CoreHospitalService<E, T> : DomainService<E, T>, ICoreHospitalService<E, T> where E : MedicalAppDomainHospital, new() where T : BaseHospitalSearch, new()
    {
        protected CoreHospitalService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

<<<<<<< HEAD
        protected CoreHospitalService(IUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        protected override SqlParameter[] GetSqlParameters(T baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
<<<<<<< HEAD
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
=======
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            };
            return parameters;
        }

    }
}
