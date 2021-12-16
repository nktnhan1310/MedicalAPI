﻿using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/catalogue")]
    [ApiController]
    [Description("Quản lý danh mục hệ thống")]
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
        private IPaymentHistoryService paymentHistoryService;
        private ISessionTypeService sessionTypeService;
        private IRoomExaminationService roomExaminationService;
        private IPaymentMethodService paymentMethodService;
        private IDoctorService doctorService;
        private IAllergyTypeService allergyTypeService;
        private IVaccineTypeService vaccineTypeService;
        private IDiagnoticTypeService diagnoticTypeService;
        private IServiceTypeMappingHospitalService serviceTypeMappingHospitalService;
<<<<<<< HEAD
        private IAdditionServiceDetailService additionServiceDetailService;
=======

>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

        #endregion

        #region Configuration

        protected IMapper mapper;
        protected IWebHostEnvironment env;
        protected IConfiguration configuration;

        #endregion

        public CatalogueController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration)
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
            paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
            sessionTypeService = serviceProvider.GetRequiredService<ISessionTypeService>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            allergyTypeService = serviceProvider.GetRequiredService<IAllergyTypeService>();
            vaccineTypeService = serviceProvider.GetRequiredService<IVaccineTypeService>();
            diagnoticTypeService = serviceProvider.GetRequiredService<IDiagnoticTypeService>();
            serviceTypeMappingHospitalService = serviceProvider.GetRequiredService<IServiceTypeMappingHospitalService>();
<<<<<<< HEAD
            additionServiceDetailService = serviceProvider.GetRequiredService<IAdditionServiceDetailService>();
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

            #region Configuration

            this.mapper = mapper;
            this.env = env;
            this.configuration = configuration;

            #endregion

        }

        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-country-catalogue")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        /// Lấy danh sách quận
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-district-catalogue/cityId")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-channel-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetChannelCatalogue(int? hospitalId, string searchContent)
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
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-degree-type-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDegreeTypeCatalogue(int? hospitalId, string searchContent)
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
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-notification-type-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetNotificationTypeCatalogue(int? hospitalId, string searchContent)
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
        /// Lấy thông tin danh sách loại dị ứng
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-allergy-type-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetAllergyTypeCatalogue(string searchContent)
        {
            var allergyTypes = await this.allergyTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || e.Description.ToLower().Contains(searchContent.ToLower()))
            );
            var allergyTypeModels = mapper.Map<IList<AllergyTypeModel>>(allergyTypes);
            return new AppDomainResult
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = allergyTypeModels,
            };
        }

        #region Hospital Catalogue

        /// <summary>
        /// Lấy danh sách chuẩn đoán
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-diagnotic-type-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetDiagnoticTypeCatalogue(int? hospitalId, string searchContent)
        {
            var diagnoticTypes = await this.diagnoticTypeService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DiagnoticTypeModel>>(diagnoticTypes),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách dịch vụ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-service-type-by-hospital-catalogue")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetServiceTypeCatalogueByHospital(int hospitalId)
        {
            IList<ServiceTypeModel> serviceTypeModels = new List<ServiceTypeModel>();
            var serviceTypeMappings = await this.serviceTypeMappingHospitalService.GetAsync(e => !e.Deleted && e.Active
            && e.HospitalId == hospitalId
            );
            if (serviceTypeMappings != null && serviceTypeMappings.Any())
            {
                var serviceTypeIds = serviceTypeMappings.Select(e => e.ServiceTypeId).Distinct().ToList();
                var serviceTypes = await this.serviceTypeService.GetAsync(e => !e.Deleted && e.Active && serviceTypeIds.Contains(e.Id));
                serviceTypeModels = mapper.Map<IList<ServiceTypeModel>>(serviceTypes);
                foreach (var serviceTypeModel in serviceTypeModels)
                {
                    var serviceTypeMappingCheck = serviceTypeMappings.Where(e => e.ServiceTypeId == serviceTypeModel.Id).FirstOrDefault();
                    if (serviceTypeMappingCheck != null) serviceTypeModel.IsBHYT = serviceTypeMappingCheck.IsBHYT;
                }
            }
            return new AppDomainResult()
            {
                Data = serviceTypeModels,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
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
            var additionServices = await this.additionServiceType.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId)
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
<<<<<<< HEAD
        /// Lấy danh sách chi tiết dịch vụ phát sinh
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="additionServiceIds"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-addition-service-detail-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetAdditionServiceDetailCatalogue(int? hospitalId, List<int> additionServiceIds, string searchContent)
        {
            //if (additionServiceIds == null || !additionServiceIds.Any()) throw new AppException("Vui lòng chọn loại dịch vụ");
            var additionServiceDetailInfos = await this.additionServiceDetailService.GetAsync(e => !e.Deleted
            && (!e.HospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (additionServiceIds == null || !additionServiceIds.Any() || (e.AdditionServiceId.HasValue && additionServiceIds.Contains(e.AdditionServiceId.Value))) 
            );
            IList<AdditionServiceDetailModel> additionServiceDetailModels = new List<AdditionServiceDetailModel>();
            if (additionServiceDetailInfos != null && additionServiceDetailInfos.Any())
                additionServiceDetailModels = mapper.Map<IList<AdditionServiceDetailModel>>(additionServiceDetailInfos);
            return new AppDomainResult()
            {
                Data = additionServiceDetailModels,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Lấy thông tin danh sách loại vaccine
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="targetId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-vaccine-type-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetVaccineTypeCatalogue(int? hospitalId, int? targetId, string searchContent)
        {
            var vaccineTypes = await this.vaccineTypeService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId)
            && (!targetId.HasValue || e.TargetIdValues.Contains(targetId.Value.ToString()))
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            var vaccineTypeModels = mapper.Map<IList<VaccineTypeModel>>(vaccineTypes);
            var targetTypes = Enum.GetValues(typeof(CatalogueUtilities.TargetType))
                            .Cast<CatalogueUtilities.TargetType>()
                            .Select(e => new
                            {
                                Id = (int)e,
                                Code = e.ToString(),
                                Name = 
                                (int)e == (int)CatalogueUtilities.TargetType.Child ? "Trẻ em" :
                                (int)e == (int)CatalogueUtilities.TargetType.Youth ? "Thanh thiếu niên" :
                                (int)e == (int)CatalogueUtilities.TargetType.Adult ? "Người lớn" :
                                (int)e == (int)CatalogueUtilities.TargetType.Pregnant ? "Phụ nữ mang thai" : "Người già"
                                ,
                                VaccineTypes = vaccineTypeModels.Where(x => x.TargetIds.Contains((int)e)).ToList()
                            })
                            .ToList();
            if (targetTypes != null && targetTypes.Any() && targetId.HasValue)
                targetTypes = targetTypes.Where(e => e.Id == targetId.Value).ToList();
            return new AppDomainResult()
            {
                Data = targetTypes,
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
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId)
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
        /// <param name="typeId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-catalogue/hospitalId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll }, CoreContants.DOCTOR_CATALOGUE_NAME)]
        public async Task<AppDomainResult> GetDoctorCatalogue(int? hospitalId, int? typeId, string searchContent)
        {
            var doctors = await this.doctorService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && (!typeId.HasValue || (e.TypeId == 0 && e.TypeId == 0) || e.TypeId != 0)
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