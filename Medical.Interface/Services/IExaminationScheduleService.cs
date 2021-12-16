using Medical.Entities;
using Medical.Interface.Services.Base;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IExaminationScheduleService : IDomainService<ExaminationSchedules, SearchExaminationSchedule>
    {
        Task<IList<ExaminationSchedules>> GetExaminationSchedules(SearchExaminationScheduleForm searchExaminationScheduleDetail);

        Task<PagedList<ExaminationSchedules>> GetAllExaminationSchedules(SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2);

        Task<IList<ExaminationScheduleDetails>> GetExaminationScheduleDetails(SearchExaminationScheduleForm searchExaminationScheduleDetail);

        /// <summary>
        /// Thêm mới nhanh lịch trực cho bác sĩ
        /// </summary>
        /// <param name="examinationScheduleExtensions"></param>
        /// <param name="hospitalId"></param>
        /// <param name="isImport"></param>
        /// <returns></returns>
        Task<bool> AddMultipleExaminationSchedule(IList<ExaminationScheduleExtensions> examinationScheduleExtensions
            , int hospitalId, bool isImport = false);

        /// <summary>
        /// Import danh sách ca trực của bệnh viện cho từng bác sĩ
        /// </summary>
        /// <param name="stream"></param>
<<<<<<< HEAD
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        Task<AppDomainImportResult> ImportExaminationSchedule(Stream stream, int hospitalId);

        /// <summary>
        /// XÓA THÔNG TIN CA TRỰC THEO PHÒNG TRỰC ĐƯỢC CHỌN
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        Task<bool> DeleteRoomExaminationSchedule(List<int> roomIds);
=======
        /// <param name="createdBy"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        Task<AppDomainImportResult> ImportExaminationSchedule(Stream stream, string createdBy, int hospitalId);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
    }
}
