using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAPI.Controllers
{
    [Route("api/examination-schedule")]
    [ApiController]
    [Description("Lịch khám")]
    [Authorize]
    public class ExaminationScheduleController : CoreHospitalController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>
    {
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        private readonly IHospitalService hospitalService;
        private readonly IDoctorService doctorService;
        private readonly ISpecialListTypeService specialListTypeService;
        private readonly IDoctorDetailService doctorDetailService;
        private readonly ISessionTypeService sessionTypeService;
        private readonly IConfigTimeExaminationService configTimeExaminationService;

        public ExaminationScheduleController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            this.examinationScheduleDetailService = serviceProvider.GetRequiredService<IExaminationScheduleDetailService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
            doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            sessionTypeService = serviceProvider.GetRequiredService<ISessionTypeService>();
            configTimeExaminationService = serviceProvider.GetRequiredService<IConfigTimeExaminationService>();
        }

        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }

            SearchExaminationSchedule searchExaminationSchedule = new SearchExaminationSchedule()
            {
                PageIndex = 1,
                PageSize = 1,
                OrderBy = "Id desc",
                HospitalId = LoginContext.Instance.CurrentUser.HospitalId,
                ExaminationScheduleId = id
            };
            var pagedItems = await this.domainService.GetPagedListData(searchExaminationSchedule);
                 
            //var item = await this.domainService.GetByIdAsync(id);

            if (pagedItems != null && pagedItems.Items.Any())
            {
                var item = pagedItems.Items.FirstOrDefault();

                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<ExaminationScheduleModel>(item);
                    var examinationScheduleDetails = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.ScheduleId == itemModel.Id);
                    if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                        itemModel.ExaminationScheduleDetails = mapper.Map<IList<ExaminationScheduleDetailModel>>(examinationScheduleDetails);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa theo bệnh viện được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-by-hospital/{hospitalId}")]
        public async Task<AppDomainResult> GetSpecialistTypeByHospital(int hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId);
            var specialistTypeModels = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = specialistTypeModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-by-hospital/{hospitalId}")]
        public async Task<AppDomainResult> GetDoctorByHospital(int hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var doctorByHospitals = await this.doctorService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId);
            var doctorModels = mapper.Map<IList<DoctorModel>>(doctorByHospitals);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = doctorModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa của bác sĩ được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-by-doctor/hospitalId/{doctorId}")]
        public async Task<AppDomainResult> GetSpecialistTypeByDoctor(int? hospitalId, int doctorId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            List<int> specialistTypeIds = new List<int>();
            var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.Active && e.DoctorId == doctorId);
            if(doctorDetails != null && doctorDetails.Any())
                specialistTypeIds = doctorDetails.Select(e => e.SpecialistTypeId).ToList();
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active 
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && specialistTypeIds.Contains(e.Id));
            var specialistTypeModels = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = specialistTypeModels
            };

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin bác sĩ theo chuyên khoa được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="specialistTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-by-specialist-type/hospitalId/{specialistTypeId}")]
        public async Task<AppDomainResult> GetDoctorBySpecialistType(int? hospitalId, int specialistTypeId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            List<int> doctorIds = new List<int>();
            var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.Active && e.SpecialistTypeId == specialistTypeId);
            if (doctorDetails != null && doctorDetails.Any())
                doctorIds = doctorDetails.Select(e => e.DoctorId).ToList();
            if(doctorIds.Any())
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
                var doctorBySpecialistTypes = await this.doctorService.GetAsync(e => !e.Deleted && e.Active 
                && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
                && doctorIds.Contains(e.Id));
                var doctorModels = mapper.Map<IList<DoctorModel>>(doctorBySpecialistTypes);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = doctorModels
                };
            }
            return appDomainResult;
        }


        #region Examination Schedule Details

        /// <summary>
        /// Lấy danh sách buổi khám
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-session-type")]
        public async Task<AppDomainResult> GetSessionType()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var sessionTypes = await this.sessionTypeService.GetAsync(e => !e.Deleted && e.Active);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<SessionTypeModel>>(sessionTypes)
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin cấu hình ca khám theo buổi khám
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="sesstionTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination/hospitalId/sessionTypeId")]
        public async Task<AppDomainResult> GetConfigTimeExamination(int? hospitalId, int? sesstionTypeId)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            AppDomainResult appDomainResult = new AppDomainResult();
            var configTimeExaminations = await this.configTimeExaminationService.GetAsync(e => !e.Deleted 
            && e.Active 
            && (!sesstionTypeId.HasValue || e.SessionId == sesstionTypeId)
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            );
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<ConfigTimeExamniationModel>>(configTimeExaminations)
            };

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin chi tiết ca
        /// </summary>
        /// <param name="examinationScheduleId"></param>
        /// <returns></returns>
        [HttpGet("get-examination-schedule-details/{examinationScheduleId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetExaminationScheduleDetails(int examinationScheduleId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var examinationSchedules = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.ScheduleId == examinationScheduleId);
            var examinationScheduleModels = mapper.Map<IList<ExaminationScheduleDetails>>(examinationSchedules);
            appDomainResult.Success = true;
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Data = examinationScheduleModels;

            return appDomainResult;
        }

        #endregion



    }
}
