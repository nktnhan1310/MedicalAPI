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
    [Route("api/medical-bill")]
    [ApiController]
    [Description("Quản lý đơn thuốc")]
    [Authorize]
    public class MedicalBillController : BaseController
    {
        private readonly IMedicalBillService medicalBillService;
        private readonly IMomoPaymentService momoPaymentService;
        private readonly IMomoConfigurationService momoConfigurationService;
        private readonly IPaymentMethodService paymentMethodService;

        public MedicalBillController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            medicalBillService = serviceProvider.GetRequiredService<IMedicalBillService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn thuốc
        /// </summary>
        /// <param name="medicalBillId"></param>
        /// <returns></returns>
        [HttpGet("get-medical-bill-info/{medicalBillId}")]
        public async Task<AppDomainResult> GetMedicalBillInfo(int? medicalBillId)
        {
            if (!medicalBillId.HasValue || medicalBillId.Value <= 0)
                throw new AppException("Không tìm thấy thông tin đơn thuốc");
            SearchMedicalBill searchMedicalBill = new SearchMedicalBill()
            {
                PageIndex = 1,
                PageSize = 1,
                OrderBy = "Id desc",
                UserId = LoginContext.Instance.CurrentUser.UserId,
                MedicalBillId = medicalBillId
            };
            var pagedData = await this.medicalBillService.GetPagedListData(searchMedicalBill);
            if(pagedData == null || !pagedData.Items.Any())
                throw new AppException("Không tìm thấy thông tin đơn thuốc");
            return new AppDomainResult()
            {
                Data = mapper.Map<MedicalBillModel>(pagedData.Items.FirstOrDefault()),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách phân trang đơn thanh toán thuốc
        /// </summary>
        /// <param name="searchMedicalBill"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPagedData([FromQuery] SearchMedicalBill searchMedicalBill)
        {
            searchMedicalBill.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedData = await this.medicalBillService.GetPagedListData(searchMedicalBill);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<MedicalBillModel>>(pagedData),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật trạng thái toa thuốc
        /// </summary>
        /// <param name="updateMedicalBillStatus"></param>
        /// <returns></returns>
        [HttpPost("update-medical-bill-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.View, CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalBillStatus([FromBody] UpdateMedicalBillStatus updateMedicalBillStatus)
        {
            MomoResponseModel momoResponseModel = null;
            updateMedicalBillStatus.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            // KIỂM TRA TRẠNG THÁI HIỆN TẠI VỚI TRẠNG THÁI CẬP NHẬT PHIẾU
            string checkMessage = await this.medicalBillService.GetCheckStatusMessage(updateMedicalBillStatus.MedicalBillId, updateMedicalBillStatus.Status);
            if (!string.IsNullOrEmpty(checkMessage)) throw new AppException(checkMessage);

            // KIỂM TRA CÓ THANH TOÁN HAY KHÔNG?
            if (updateMedicalBillStatus.PaymentMethodId.HasValue && updateMedicalBillStatus.PaymentMethodId.Value > 0)
            {
                var paymentMethodInfos = await this.paymentMethodService.GetAsync(e => !e.Deleted && e.Active && e.Id == updateMedicalBillStatus.PaymentMethodId.Value);
                if (paymentMethodInfos != null && paymentMethodInfos.Any())
                {
                    var paymentMethodInfo = paymentMethodInfos.FirstOrDefault();
                    // THANH TOÁN QUA MOMO => Trạng thái chờ xác nhận => Chờ thanh toán thành công => Cập nhật trạng thái
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.MOMO.ToString())
                    {
                        momoResponseModel = await GetResponseMomoPayment(updateMedicalBillStatus);
                    }
                }
            }
            // THANH TOÁN QUA APP HOẶC COD => Cập nhật trạng thái phiếu => Chờ admin xác nhận
            bool success = await this.medicalBillService.UpdateMedicalBillStatus(updateMedicalBillStatus);
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
        /// <param name="updateMedicalBillStatus"></param>
        /// <returns></returns>
        private async Task<MomoResponseModel> GetResponseMomoPayment(UpdateMedicalBillStatus updateMedicalBillStatus)
        {
            MomoResponseModel momoResponseModel = null;
            MomoConfigurations momoConfiguration = new MomoConfigurations();
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            var momoConfigurationInfos = await this.momoConfigurationService.GetAsync(e => !e.Deleted && e.Active);
            if (momoConfigurationInfos != null && momoConfigurationInfos.Any())
            {
                momoConfiguration = momoConfigurationInfos.FirstOrDefault();
                string orderInfo = "test";
                string amount = updateMedicalBillStatus.TotalPrice.HasValue ? updateMedicalBillStatus.TotalPrice.Value.ToString() : "0";
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
                        Amount = amount,
                        RequestId = requestId,
                        OrderId = orderid,
                        OrderInfo = orderInfo,
                        Signature = signature,
                        MedicalBillId = updateMedicalBillStatus.MedicalBillId,
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
