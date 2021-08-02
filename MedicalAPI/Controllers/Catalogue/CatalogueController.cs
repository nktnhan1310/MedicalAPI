using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/catalogue")]
    [ApiController]
    [Description("Quản lý danh mục")]
    [Authorize]
    public class CatalogueController : ControllerBase
    {

        #region Services

        private IWardService wardService;
        private IDistrictService districtService;
        private ICountryService countryService;
        private ICityService cityService;
        private IAdditionServiceType additionServiceType;
        private INationService nationService;
        private ISpecialListTypeService specialListTypeService;
        private IServiceTypeService serviceTypeService;
        private IDegreeTypeService degreeTypeService;
        private IJobService jobService;
        private IHospitalService hospitalService;
        private IChannelService channelService;
        private INotificationTypeService notificationTypeService;
        private ISessionTypeService sessionTypeService;
        private IRoomExaminationService roomExaminationService;
        private IPaymentMethodService paymentMethodService;
        private ISystemConfigFeeService systemConfigFeeService;
        private IDoctorService doctorService;
        private IMedicalRecordService medicalRecordService;

        #endregion

        #region Configuration

        protected IMapper mapper;
        protected IConfiguration configuration;

        #endregion

        public CatalogueController(IServiceProvider serviceProvider, IMapper mapper, IConfiguration configuration)
        {
            wardService = serviceProvider.GetRequiredService<IWardService>();
            districtService = serviceProvider.GetRequiredService<IDistrictService>();
            cityService = serviceProvider.GetRequiredService<ICityService>();
            countryService = serviceProvider.GetRequiredService<ICountryService>();
            additionServiceType = serviceProvider.GetRequiredService<IAdditionServiceType>();
            nationService = serviceProvider.GetRequiredService<INationService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();

            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
            serviceTypeService = serviceProvider.GetRequiredService<IServiceTypeService>();
            degreeTypeService = serviceProvider.GetRequiredService<IDegreeTypeService>();
            jobService = serviceProvider.GetRequiredService<IJobService>();
            channelService = serviceProvider.GetRequiredService<IChannelService>();
            notificationTypeService = serviceProvider.GetRequiredService<INotificationTypeService>();
            sessionTypeService = serviceProvider.GetRequiredService<ISessionTypeService>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
            systemConfigFeeService = serviceProvider.GetRequiredService<ISystemConfigFeeService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();

            #region Configuration

            this.mapper = mapper;
            this.configuration = configuration;

            #endregion

        }

        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-country-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetCountryCatalogue(string searchContent)
        {
            var countries = await this.countryService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<CountryModel>>(countries),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách thành phố
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-city-catalogue/countryId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetCityCatalogue(int? countryId, string searchContent)
        {
            var cities = await this.cityService.GetAsync(e => !e.Deleted && e.Active
            && (!countryId.HasValue || e.CountryId == countryId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<CityModel>>(cities),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Danh sách ngày trong tuần
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-day-of-week-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDayOfWeekCatalogue()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await Task.Run(() =>
            {
                var listDayOfWeek = new List<object>()
                {
                    new
                    {
                        Id = 1,
                        Name = "CN"
                    },
                    new
                    {
                        Id = 2,
                        Name = "T2"
                    },
                    new
                    {
                        Id = 3,
                        Name = "T3"
                    },
                    new
                    {
                        Id = 4,
                        Name = "T4"
                    },
                    new
                    {
                        Id = 5,
                        Name = "T5"
                    },
                    new
                    {
                        Id = 6,
                        Name = "T6"
                    },
                    new
                    {
                        Id = 7,
                        Name = "T7"
                    },
                };
                appDomainResult = new AppDomainResult()
                {
                    Data = Task.FromResult(listDayOfWeek),
                    ResultCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            });
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách quận
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-district-catalogue/cityId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDistrictCatalogue(int? cityId, string searchContent)
        {
            var districts = await this.districtService.GetAsync(e => !e.Deleted && e.Active
            && (!cityId.HasValue || e.CityId == cityId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DistrictModel>>(districts),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách phường
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="districtid"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-ward-catalogue/cityId/districtId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetWardCatalogue(int? cityId, int? districtid, string searchContent)
        {
            var wards = await this.wardService.GetAsync(e => !e.Deleted && e.Active
            && (!cityId.HasValue || e.CityId == cityId.Value)
            && (!districtid.HasValue || e.DistrictId == districtid.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<WardModel>>(wards),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách dân tộc
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-nation-catalogue/countryId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetNationCatalogue(int? countryId, string searchContent)
        {
            var nations = await this.nationService.GetAsync(e => !e.Deleted && e.Active
            && (!countryId.HasValue || e.CountryId == countryId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<NationModel>>(nations),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách công việc
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-job-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetJobCatalogue(string searchContent)
        {
            var jobs = await this.jobService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<JobModel>>(jobs),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách dịch vụ của bệnh viện
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-service-type-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetServiceTypeCatalogue(string searchContent)
        {
            var serviceTypes = await this.serviceTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<ServiceTypeModel>>(serviceTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách bệnh viện
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-hospital-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetHospitalCatalogue(string searchContent)
        {
            var hospitals = await this.hospitalService.GetAsync(e => !e.Deleted && e.Active
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.Id == LoginContext.Instance.CurrentUser.HospitalId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<HospitalModel>>(hospitals),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách kênh đăng kí
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-channel-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetChannelCatalogue(string searchContent)
        {
            var channels = await this.channelService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<ChannelModel>>(channels),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách chức vị của bác sĩ
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-degree-type-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDegreeTypeCatalogue(string searchContent)
        {
            var degreeTypes = await this.degreeTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DegreeTypeModel>>(degreeTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách loại thông báo
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-notification-type-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetNotificationTypeCatalogue(string searchContent)
        {
            var notificationTypes = await this.notificationTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<NotificationTypeModel>>(notificationTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách buổi khám
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-session-type-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetSessionTypeCatalogue(string searchContent)
        {
            var sessionTypes = await this.sessionTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<SessionTypeModel>>(sessionTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách phương thức thanh toán
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-payment-method-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPaymentMethodCatalogue(string searchContent)
        {
            var paymentMethods = await this.paymentMethodService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<PaymentMethodModel>>(paymentMethods),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin phí tiện ích từ cấu hình hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-system-config-fee")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetSystemConfigFee()
        {
            SystemConfigFeeModel systemConfigFeeModel = new SystemConfigFeeModel();
            var systemConfigFeeInfos = await this.systemConfigFeeService.GetAsync(e => !e.Deleted && e.Active);
            if (systemConfigFeeInfos != null && systemConfigFeeInfos.Any())
                systemConfigFeeModel = mapper.Map<SystemConfigFeeModel>(systemConfigFeeInfos.FirstOrDefault());
            return new AppDomainResult()
            {
                Data = systemConfigFeeModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #region Hospital Catalogue

        /// <summary>
        /// Lấy danh sách hồ sơ người bệnh
        /// </summary>
        /// <param name="searchMedicalRecord"></param>
        /// <returns></returns>
        [HttpGet("get-medical-record-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetMedicalRecordCatalogue([FromQuery] SearchMedicalRecord searchMedicalRecord)
        {
            //if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
            //    searchMedicalRecord.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            if (searchMedicalRecord.PageIndex <= 0 || searchMedicalRecord.PageSize <= 0)
            {
                searchMedicalRecord.PageIndex = 1;
                searchMedicalRecord.PageSize = int.MaxValue;
            }
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                PagedList<MedicalRecords> pagedData = await this.medicalRecordService.GetPagedListData(searchMedicalRecord);
                PagedList<MedicalRecordModel> pagedDataModel = mapper.Map<PagedList<MedicalRecordModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }


        /// <summary>
        /// Lấy danh sách dịch vụ phát sinh của bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-addition-service-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetAdditionServiceTypeCatalogue(int? hospitalId, string searchContent)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var additionServices = await this.additionServiceType.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<AdditionServiceModel>>(additionServices),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách chuyên khoa của bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetSpecialistTypeCatalogue(int? hospitalId, string searchContent)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách Phòng khám của bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-room-examination-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetRoomExaminationCatalogue(int? hospitalId, string searchContent)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var roomExaminations = await this.roomExaminationService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<RoomExaminationModel>>(roomExaminations),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDoctorCatalogue(int? hospitalId, string searchContent)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var doctors = await this.doctorService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.FirstName.Contains(searchContent)
            || e.LastName.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DoctorModel>>(doctors),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }



        #endregion

    }
}
