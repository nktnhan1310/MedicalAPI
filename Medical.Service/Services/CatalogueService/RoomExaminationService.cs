using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class RoomExaminationService : CatalogueHospitalService<RoomExaminations, SearchHopitalExtension>, IRoomExaminationService
    {
        public RoomExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<string> GetExistItemMessage(RoomExaminations item)
        {
            string result = string.Empty;
            bool isExistCode = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.HospitalId == item.Id && x.Code == item.Code);
            if (isExistCode)
                return "Mã đã tồn tại!";
            return result;
        }

        /// <summary>
        /// Lấy thông tin trực theo phòng
        /// </summary>
        /// <param name="searchHopitalExtension"></param>
        /// <returns></returns>
        public async Task<IList<ExaminationScheduleDetails>> GetRoomDetail(SearchHopitalExtension searchHopitalExtension)
        {
            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();
            DateTime baseDate = DateTime.Now;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
            var thisWeekEnd = thisWeekStart.AddDays(6);
            if (!searchHopitalExtension.FromExaminationDate.HasValue)
                searchHopitalExtension.FromExaminationDate = thisWeekStart;
            if (!searchHopitalExtension.ToExaminationDate.HasValue)
                searchHopitalExtension.ToExaminationDate = thisWeekEnd;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchHopitalExtension.HospitalId),
                new SqlParameter("@RoomExaminationId", searchHopitalExtension.RoomExaminationId),
                new SqlParameter("@FromExaminationDate", searchHopitalExtension.FromExaminationDate),
                new SqlParameter("@ToExaminationDate", searchHopitalExtension.ToExaminationDate),
            };
            examinationScheduleDetails = await unitOfWork.Repository<ExaminationScheduleDetails>().ExcuteStoreAsync("RoomDetail_GetInfo", sqlParameters);
            return examinationScheduleDetails;
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
