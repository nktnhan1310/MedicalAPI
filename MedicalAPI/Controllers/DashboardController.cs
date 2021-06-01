﻿using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Medical.Utilities.CatalogueUtilities;

namespace MedicalAPI.Controllers
{
    [Route("api/dash-board")]
    [ApiController]
    [Description("Dashboard")]
    public class DashboardController : ControllerBase
    {
        protected readonly ILogger<DashboardController> logger;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMapper mapper;
        protected IWebHostEnvironment env;
        private readonly IDashBoardService dashBoardService;
        private readonly IServiceTypeService serviceTypeService;
        public DashboardController(IServiceProvider serviceProvider, ILogger<DashboardController> logger, IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
            dashBoardService = serviceProvider.GetRequiredService<IDashBoardService>();
            serviceTypeService = serviceProvider.GetRequiredService<IServiceTypeService>();
        }

        /// <summary>
        /// Lấy tổng lịch hẹn mới theo ngày/tháng/năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-examination-new")]
        public async Task<AppDomainResult> GetExaminationNew()
        {
            DashBoardRequest dashboardRequest = new DashBoardRequest()
            {
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                ExaminationDate = DateTime.Now,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year
            };
            var listStatus = new List<int>() { (int)ExaminationStatus.New, (int)ExaminationStatus.WaitConfirm, (int)ExaminationStatus.Confirmed };
            dashboardRequest.StatusList = string.Join(",", listStatus);
            var dashBoardResponse = await this.dashBoardService.GetTotalExaminationByRequest(dashboardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalExaminationByDate = dashBoardResponse.TotalExaminationFormByDate,
                    TotalExaminationByMonth = dashBoardResponse.TotalExaminationFormByMonth,
                    TotalExaminationByYear = dashBoardResponse.TotalExaminationFormByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng lịch tái khám mới theo ngày/tháng/năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-re-examination")]
        public async Task<AppDomainResult> GetReExamination()
        {
            DashBoardRequest dashboardRequest = new DashBoardRequest()
            {
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                ExaminationDate = DateTime.Now,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year
            };
            var listStatus = new List<int>() { (int)ExaminationStatus.ConfirmedReExamination, (int)ExaminationStatus.WaitReExamination };
            dashboardRequest.StatusList = string.Join(",", listStatus);
            var dashBoardResponse = await this.dashBoardService.GetTotalExaminationByRequest(dashboardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalExaminationByDate = dashBoardResponse.TotalExaminationFormByDate,
                    TotalExaminationByMonth = dashBoardResponse.TotalExaminationFormByMonth,
                    TotalExaminationByYear = dashBoardResponse.TotalExaminationFormByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng lịch hủy theo ngày/tháng/năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-examination-canceled")]
        public async Task<AppDomainResult> GetCanceledExamination()
        {
            DashBoardRequest dashboardRequest = new DashBoardRequest()
            {
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                ExaminationDate = DateTime.Now,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year
            };
            var listStatus = new List<int>() { (int)ExaminationStatus.Canceled };
            dashboardRequest.StatusList = string.Join(",", listStatus);
            var dashBoardResponse = await this.dashBoardService.GetTotalExaminationByRequest(dashboardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalExaminationByDate = dashBoardResponse.TotalExaminationFormByDate,
                    TotalExaminationByMonth = dashBoardResponse.TotalExaminationFormByMonth,
                    TotalExaminationByYear = dashBoardResponse.TotalExaminationFormByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng số người đã đến khám bệnh viện ngày/tháng/năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-examination-total-user")]
        public async Task<AppDomainResult> GetTotalUserExamination()
        {
            DashBoardRequest dashBoardRequest = new DashBoardRequest()
            {
                ExaminationDate = DateTime.Now,
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year
            };
            var dashBoardResponse = await this.dashBoardService.GetTotalUserExamination(dashBoardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalUserByDate = dashBoardResponse.TotalUserByDate,
                    TotalUserByMonth = dashBoardResponse.TotalUserByMonth,
                    TotalUserByYear = dashBoardResponse.TotalUserByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng lịch khám theo chuyên khoa ngày/tháng/năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-examination-by-service")]
        public async Task<AppDomainResult> GetExaminationByService()
        {
            var specialistServiceTypeResults = await this.serviceTypeService.GetAsync(e => e.Code == CatalogueUtilities.ServiceType.KCK.ToString());
            int? serviceTypeId = null;
            if (specialistServiceTypeResults != null && specialistServiceTypeResults.Any())
                serviceTypeId = specialistServiceTypeResults.FirstOrDefault().Id;
            DashBoardRequest dashboardRequest = new DashBoardRequest()
            {
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                ExaminationDate = DateTime.Now,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year,
                ServiceTypeId = serviceTypeId,
            };
            var dashBoardResponse = await this.dashBoardService.GetTotalExaminationByRequest(dashboardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalExaminationByDate = dashBoardResponse.TotalExaminationFormByDate,
                    TotalExaminationByMonth = dashBoardResponse.TotalExaminationFormByMonth,
                    TotalExaminationByYear = dashBoardResponse.TotalExaminationFormByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng số user của hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-total-user-system")]
        public async Task<AppDomainResult> GetTotalUserSystem()
        {
            DashBoardRequest dashBoardRequest = new DashBoardRequest();
            var dashBoardResponse = await this.dashBoardService.GetTotalUser(dashBoardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalUserByDate = dashBoardResponse.TotalUserByDate,
                    TotalUserByMonth = dashBoardResponse.TotalUserByMonth,
                    TotalUserByYear = dashBoardResponse.TotalUserByYear,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tổng số người active cùng 1 thời gian
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-total-user-active")]
        public async Task<AppDomainResult> GetTotalUserActive()
        {
            DashBoardRequest dashBoardRequest = new DashBoardRequest();
            dashBoardRequest.IsCheckActiveUser = true;
            var dashBoardResponse = await this.dashBoardService.GetTotalUser(dashBoardRequest);
            return new AppDomainResult()
            {
                Data = new
                {
                    TotalUserActive = dashBoardResponse.TotalUserActive,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Tổng tiền thanh toán qua App/COD theo ngày tháng năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-total-payment")]
        public async Task<AppDomainResult> GetTotalPayment()
        {
            DashBoardRequest dashBoardRequest = new DashBoardRequest()
            {
                HospitalId = LoginContext.Instance.CurrentUser != null ? LoginContext.Instance.CurrentUser.HospitalId : null,
                ExaminationDate = DateTime.Now,
                MonthValue = DateTime.Now.Month,
                YearValue = DateTime.Now.Year,
            };
            var dashBoardResponse = await this.dashBoardService.GetTotalPaymentSystem(dashBoardRequest);

            return new AppDomainResult()
            {
                Data = new
                {
                    //---- THANH TOÁN QUA APP
                    TotalAppPriceByDate = dashBoardResponse.TotalAppPriceByDate,
                    TotalAppPriceByDateDisplay = dashBoardResponse.TotalAppPriceByDateDisplay,
                    TotalAppPriceByMonth = dashBoardResponse.TotalAppPriceByMonth,
                    TotalAppPriceByMonthDisplay = dashBoardResponse.TotalAppPriceByMonthDisplay,
                    TotalAppPriceByYear = dashBoardResponse.TotalAppPriceByYear,
                    TotalAppPriceByYearDisplay = dashBoardResponse.TotalAppPriceByYearDisplay,
                    //---- THANH TOÁN QUA COD
                    TotalCODPriceByDate = dashBoardResponse.TotalCODPriceByDate,
                    TotalCODPriceByDateDisplay = dashBoardResponse.TotalCODPriceByDateDisplay,
                    TotalCODPriceByMonth = dashBoardResponse.TotalCODPriceByMonth,
                    TotalCODPriceByMonthDisplay = dashBoardResponse.TotalCODPriceByMonthDisplay,
                    TotalCODPriceByYear = dashBoardResponse.TotalCODPriceByYear,
                    TotalCODPriceByYearDisplay = dashBoardResponse.TotalCODPriceByYearDisplay,
                },
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


    }
}