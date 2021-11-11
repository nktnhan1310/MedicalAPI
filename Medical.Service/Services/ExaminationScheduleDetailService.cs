using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ExaminationScheduleDetailService : DomainService<ExaminationScheduleDetails, SearchExaminationScheduleDetail>, IExaminationScheduleDetailService
    {
        public ExaminationScheduleDetailService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationScheduleDetail_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationScheduleDetail baseSearch)
        {
            SqlParameter[] parameters =
           {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@RoomExaminationId", baseSearch.RoomExaminationId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@DoctorId", baseSearch.DoctorId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@SessionTypeId", baseSearch.SessionTypeId),
                new SqlParameter("@SpecialistTypeId", baseSearch.SpecialistTypeId),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Lấy danh sách phòng khám theo chi tiết ca trực
        /// </summary>
        /// <param name="searchRoomExaminationSchedule"></param>
        /// <returns></returns>
        public async Task<PagedList<ExaminationScheduleDetails>> GetRoomExaminationScheduleDetailInfo(SearchRoomExaminationSchedule searchRoomExaminationSchedule)
        {
            PagedList<ExaminationScheduleDetails> pagedList = new PagedList<ExaminationScheduleDetails>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", searchRoomExaminationSchedule.PageIndex),
                new SqlParameter("@PageSize", searchRoomExaminationSchedule.PageSize),

                new SqlParameter("@HospitalId", searchRoomExaminationSchedule.HospitalId),
                new SqlParameter("@RoomExaminationId", searchRoomExaminationSchedule.RoomExaminationId),
                new SqlParameter("@SpecialistTypeId", searchRoomExaminationSchedule.SpecialistTypeId),
                new SqlParameter("@ExaminationDate", searchRoomExaminationSchedule.ExaminationDate),
                new SqlParameter("@SessionTypeId", searchRoomExaminationSchedule.SessionTypeId),
                new SqlParameter("@SearchContent", searchRoomExaminationSchedule.SearchContent),
                new SqlParameter("@OrderBy", searchRoomExaminationSchedule.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };

            pagedList = await this.unitOfWork.Repository<ExaminationScheduleDetails>().ExcuteQueryPagingAsync("RoomExamination_GetPagingData_V2", parameters);
            pagedList.PageIndex = searchRoomExaminationSchedule.PageIndex;
            pagedList.PageSize = searchRoomExaminationSchedule.PageSize;

            return pagedList;
        }

    }
}
