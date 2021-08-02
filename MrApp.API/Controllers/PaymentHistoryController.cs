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

namespace MrApp.API.Controllers
{
    [Route("api/payment-history")]
    [ApiController]
    [Authorize]
    [Description("Lịch sử thanh toán")]
    public class PaymentHistoryController : ControllerBase
    {
        private IPaymentHistoryService paymentHistoryService;

        #region Configuration

        protected IMapper mapper;
        protected IConfiguration configuration;

        #endregion

        public PaymentHistoryController(IServiceProvider serviceProvider, IMapper mapper, IConfiguration configuration)
        {
            paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();

            #region Configuration

            this.mapper = mapper;
            this.configuration = configuration;

            #endregion
        }

        /// <summary>
        /// Lấy tất cả danh sách lịch sử thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppDomainResult> Get()
        {
            SearchPaymentHistory searchPaymentHistory = new SearchPaymentHistory()
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                OrderBy = "Created desc",
                UserId = LoginContext.Instance.CurrentUser.UserId
            };
            var pagedList = await this.paymentHistoryService.GetPagedListData(searchPaymentHistory);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<PaymentHistoryModel>>(pagedList),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách phân trang lịch sử thanh toán
        /// </summary>
        /// <param name="searchPaymentHistory"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        public async Task<AppDomainResult> GetPagedPaymentHistory([FromQuery] SearchPaymentHistory searchPaymentHistory)
        {
            searchPaymentHistory.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedList = await this.paymentHistoryService.GetPagedListData(searchPaymentHistory);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<PaymentHistoryModel>>(pagedList),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
