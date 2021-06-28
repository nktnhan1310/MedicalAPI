using AutoMapper;
using Medical.Extensions;
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
    [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
    public class CatalogueController : BaseController
    {
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


        public CatalogueController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
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


        }

        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-country-catalogue")]
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

        #region Hospital Catalogue

        /// <summary>
        /// Lấy danh sách dịch vụ phát sinh của bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-addition-service-catalogue/hospitalId")]
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
        /// Lấy danh sách chuyên khoa của bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-catalogue/hospitalId")]
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

        #endregion

    }
}
