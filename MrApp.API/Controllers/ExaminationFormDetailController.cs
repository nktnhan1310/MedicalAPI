using AutoMapper;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/examination-form-detail")]
    [ApiController]
    [Description("Quản lý thanh toán dịch vụ phát sinh")]
    [Authorize]
    public class ExaminationFormDetailController : BaseController
    {
        private IExaminationFormDetailService examinationFormDetailService;
        private IMomoConfigurationService momoConfigurationService;
        private IMomoPaymentService momoPaymentService;
        private IPaymentMethodService paymentMethodService;

        public ExaminationFormDetailController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
        }

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ phát sinh
        /// </summary>
        /// <param name="examinationFormDetailId"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-detail-info/{examinationFormDetailId}")]
        public async Task<AppDomainResult> GetExaminationFormDetailInfo(int? examinationFormDetailId)
        {
            if (!examinationFormDetailId.HasValue || examinationFormDetailId.Value <= 0)
                throw new AppException("Không có thông tin dịch vụ");
            SearchExaminationFormDetail searchExaminationFormDetail = new SearchExaminationFormDetail()
            {
                PageIndex = 1,
                PageSize = 1,
                OrderBy = "Id desc",
                UserId = LoginContext.Instance.CurrentUser.UserId,
                ExaminationFormDetailId = examinationFormDetailId
            };
            var pagedData = await this.examinationFormDetailService.GetPagedListData(searchExaminationFormDetail);
            if (pagedData == null || !pagedData.Items.Any())
                throw new AppException("Không có thông tin dịch vụ");
            return new AppDomainResult()
            {
                Data = mapper.Map<ExaminationFormDetailModel>(pagedData.Items.FirstOrDefault()),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách phân trang dịch vụ phát sinh
        /// </summary>
        /// <param name="searchExaminationFormDetail"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPagedData([FromQuery] SearchExaminationFormDetail searchExaminationFormDetail)
        {
            searchExaminationFormDetail.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedData = await this.examinationFormDetailService.GetPagedListData(searchExaminationFormDetail);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<ExaminationFormDetailModel>>(pagedData),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán cho dịch vụ phát sinh
        /// </summary>
        /// <returns></returns>
        [HttpPost("update-examination-form-detail-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateExaminationFormDetailStatus([FromBody] UpdateExaminationFormDetailStatus updateExaminationFormDetailStatus)
        {
            MomoResponseModel momoResponseModel = null;
            updateExaminationFormDetailStatus.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            // KIỂM TRA TRẠNG THÁI HIỆN TẠI VỚI TRẠNG THÁI CẬP NHẬT PHIẾU
            string checkMessage = await this.examinationFormDetailService.GetCheckStatusMessage(updateExaminationFormDetailStatus.ExaminationFormDetailId, updateExaminationFormDetailStatus.Status);
            if (!string.IsNullOrEmpty(checkMessage)) throw new AppException(checkMessage);

            // KIỂM TRA CÓ THANH TOÁN HAY KHÔNG?
            if (updateExaminationFormDetailStatus.PaymentMethodId.HasValue)
            {
                var paymentMethodInfos = await this.paymentMethodService.GetAsync(e => !e.Deleted && e.Active && e.Id == updateExaminationFormDetailStatus.PaymentMethodId.Value);
                if (paymentMethodInfos != null && paymentMethodInfos.Any())
                {
                    var paymentMethodInfo = paymentMethodInfos.FirstOrDefault();
                    // THANH TOÁN QUA MOMO => Trạng thái chờ xác nhận => Chờ thanh toán thành công => Cập nhật trạng thái
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.MOMO.ToString())
                    {
                        momoResponseModel = await GetResponseMomoPayment(updateExaminationFormDetailStatus);
                    }
                }
            }
            // THANH TOÁN QUA APP HOẶC COD => Cập nhật trạng thái phiếu => Chờ admin xác nhận
            bool success = await this.examinationFormDetailService.UpdateExaminationFormDetailStatus(updateExaminationFormDetailStatus);
            return new AppDomainResult()
            {
                Data = momoResponseModel,
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Trả lại thông tin phản hồi từ momo
        /// </summary>
        /// <param name="updateExaminationFormDetailStatus"></param>
        /// <returns></returns>
        private async Task<MomoResponseModel> GetResponseMomoPayment(UpdateExaminationFormDetailStatus updateExaminationFormDetailStatus)
        {
            MomoResponseModel momoResponseModel = null;
            MomoConfigurations momoConfiguration = new MomoConfigurations();
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            var momoConfigurationInfos = await this.momoConfigurationService.GetAsync(e => !e.Deleted && e.Active);
            if (momoConfigurationInfos != null && momoConfigurationInfos.Any())
            {
                momoConfiguration = momoConfigurationInfos.FirstOrDefault();
                string orderInfo = "test";
                string amount = updateExaminationFormDetailStatus.TotalPrice.HasValue ? (updateExaminationFormDetailStatus.TotalPrice.Value.ToString()) : "0";
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                momoConfiguration.PartnerCode + "&accessKey=" +
                momoConfiguration.AccessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                momoConfiguration.ReturnUrlWebApp + "&notifyUrl=" +
                momoConfiguration.NotifyUrl + "&extraData=" +
                extraData;

                MomoUtilities crypto = new MomoUtilities();
                //sign signature SHA256
                string signature = crypto.SignSHA256(rawHash, momoConfiguration.SecretKey);

                //build body json request
                JObject message = new JObject
                {
                    { "partnerCode", momoConfiguration.PartnerCode },
                    { "accessKey", momoConfiguration.AccessKey },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderid },
                    { "orderInfo", orderInfo },
                    { "returnUrl", momoConfiguration.ReturnUrlWebApp },
                    { "notifyUrl", momoConfiguration.NotifyUrl },
                    { "extraData", extraData },
                    { "requestType", "captureMoMoWallet" },
                    { "signature", signature }
                };
                string responseFromMomo = crypto.SendPaymentRequest(endpoint, message.ToString());
                momoResponseModel = JsonConvert.DeserializeObject<MomoResponseModel>(responseFromMomo);
                if (momoResponseModel != null && momoResponseModel.errorCode == 0)
                {
                    MomoPayments momoPayments = new MomoPayments()
                    {
                        Created = DateTime.Now,
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        Active = true,
                        Deleted = false,
                        Amount = Convert.ToInt64(amount),
                        RequestId = requestId,
                        OrderId = orderid,
                        OrderInfo = orderInfo,
                        Signature = signature,
                        ExaminationFormDetailId = updateExaminationFormDetailStatus.ExaminationFormDetailId
                    };
                    bool success = await this.momoPaymentService.CreateAsync(momoPayments);
                    if (!success) throw new AppException("Không lưu được thông tin thanh toán");
                }
            }
            else throw new AppException("Không lấy được cấu hình thanh toán momo");
            return momoResponseModel;
        }

    }
}
