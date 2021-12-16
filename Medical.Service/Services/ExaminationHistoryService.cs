using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Medical.Service
{
    public class ExaminationHistoryService : DomainService<ExaminationHistories, SearchExaminationFormHistory>, IExaminationHistoryService
    {
        public ExaminationHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationFormHistory_GetPagingData";
        }

        /// <summary>
        /// Lấy thông tin parameter sql store
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        protected override SqlParameter[] GetSqlParameters(SearchExaminationFormHistory baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@RecordId", baseSearch.RecordId),
                new SqlParameter("@DoctorId", baseSearch.DoctorId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@ServiceTypeId", baseSearch.ServiceTypeId),
                new SqlParameter("@StatusIds", baseSearch.StatusIds != null && baseSearch.StatusIds.Any() ? string.Join(';', baseSearch.StatusIds) : string.Empty),



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
