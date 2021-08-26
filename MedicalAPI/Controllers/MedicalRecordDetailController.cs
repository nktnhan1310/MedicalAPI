using AutoMapper;
using Medical.Core.App.Controllers;
using Medical.Entities;
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

namespace MedicalAPI.Controllers
{
    [Route("api/medical-record-detail")]
    [ApiController]
    [Description("Quản lý hồ sơ chi tiết bệnh án")]
    [Authorize]
    public class MedicalRecordDetailController : CoreHospitalController<MedicalRecordDetails, MedicalRecordDetailModel, SearchMedicalRecordDetail>
    {
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private IMedicalRecordDetailFileService medicalRecordDetailFileService;
        private IExaminationFormDetailService examinationFormDetailService;
        public MedicalRecordDetailController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<MedicalRecordDetails, MedicalRecordDetailModel, SearchMedicalRecordDetail>> logger, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy thông tin chi tiết hồ sơ bệnh án theo user
        /// </summary>
        /// <param name="recordDetailId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-record-detail-info-by-user/{recordDetailId}")]
        public async Task<IActionResult> GetRecordDetailInfoByUser(int recordDetailId)
        {
            SearchMedicalRecordDetail searchMedicalRecordDetail = new SearchMedicalRecordDetail()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Id desc",
                MedicalRecordDetailId = recordDetailId,
                //UserId = userId,
            };
            var pagedList = await this.domainService.GetPagedListData(searchMedicalRecordDetail);
            if (pagedList != null && pagedList.Items.Any())
            {
                var recordDetailInfo = pagedList.Items.FirstOrDefault();
                recordDetailInfo.MedicalRecordDetailFiles = await medicalRecordDetailFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == recordDetailInfo.Id);
                var recordDetailInfoModel = mapper.Map<MedicalRecordDetailModel>(recordDetailInfo);
                return Ok(recordDetailInfoModel);
            }
            else return NotFound("Không tìm thấy thông tin");
        }

        /// <summary>
        /// Lấy thông tin dịch vụ phát sinh
        /// </summary>
        /// <param name="searchExaminationFormDetail"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-detail")]
        public async Task<AppDomainResult> GetExaminationFormDetails([FromQuery] SearchExaminationFormDetail searchExaminationFormDetail)
        {
            searchExaminationFormDetail.UserId = LoginContext.Instance.CurrentUser.UserId;
            searchExaminationFormDetail.IsFromMedicalRecordDetail = true;
            var pagedData = await this.examinationFormDetailService.GetPagedListData(searchExaminationFormDetail);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<ExaminationFormDetailModel>>(pagedData),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
