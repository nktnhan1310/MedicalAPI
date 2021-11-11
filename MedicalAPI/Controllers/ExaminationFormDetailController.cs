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

namespace MedicalAPI.Controllers
{
    [Route("api/examination-form-detail")]
    [ApiController]
    [Authorize]
    [Description("Quản lý dịch vụ phát sinh khám bệnh")]
    public class ExaminationFormDetailController : CoreHospitalController<ExaminationFormDetails, ExaminationFormDetailModel, SearchExaminationFormDetail>
    {
        private readonly IExaminationFormDetailService examinationFormDetailService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly IMomoPaymentService momoPaymentService;
        private readonly IMomoConfigurationService momoConfigurationService;

        public ExaminationFormDetailController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<ExaminationFormDetails, ExaminationFormDetailModel, SearchExaminationFormDetail>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
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
