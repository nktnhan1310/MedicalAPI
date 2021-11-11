using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/momo-payment")]
    [ApiController]
    public class MomoPaymentController : ControllerBase
    {
        public const string SecretKey = "iLSO7piswvetiHeGv8bdPaJ02B5vxywt";
        private IMapper mapper;
        private IMomoPaymentService momoPaymentService;
        private IMomoConfigurationService momoConfigurationService;
        private IExaminationFormService examinationFormService;
        private ILogger<MomoPaymentController> logger;
        public MomoPaymentController(IServiceProvider serviceProvider, IMapper mapper, ILogger<MomoPaymentController> logger)
        {
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Thử thanh toán qua momo
        /// </summary>
        /// <returns></returns>
        [HttpGet("test-momo-payment")]
        public async Task<AppDomainResult> TestMoMoPayment()
        {
            bool success = false;

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOXX5E20210630";
            string accessKey = "2Fkmo9xwo2UZxooQ";
            //string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            string orderInfo = "test";
            string returnUrl = "https://s.mrapp.vn/api/momo-payment/get-momo-return-result";
            string notifyurl = "https://s.mrapp.vn/api/momo-payment/get-momo-notify-result";

            string amount = "1000";
            string orderid = "16111995";// Guid.NewGuid().ToString();
            string requestId = "16111995"; //Guid.NewGuid().ToString();
            string extraData = "";



            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;
            MomoUtilities crypto = new MomoUtilities();
            //sign signature SHA256
            string signature = crypto.SignSHA256(rawHash, SecretKey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
            string responseFromMomo = crypto.SendPaymentRequest(endpoint, message.ToString());
            var momoResponseModel = JsonConvert.DeserializeObject<MomoResponseModel>(responseFromMomo);
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
                };
                success = await this.momoPaymentService.CreateAsync(momoPayments);
            }

            return new AppDomainResult()
            {
                Data = momoResponseModel,
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Hoàn tiền qua momo
        /// </summary>
        /// <returns></returns>
        [HttpGet("test-momo-refund/transId")]
        public async Task<AppDomainResult> TestMomoRefund(string transId)
        {
            bool success = false;

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOXX5E20210630";
            string accessKey = "2Fkmo9xwo2UZxooQ";
            //string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            string orderInfo = "test";
            string returnUrl = "https://s.mrapp.vn/api/momo-payment/get-momo-return-result";
            string notifyurl = "https://s.mrapp.vn/api/momo-payment/get-momo-notify-result";

            string amount = "1000";
            string orderid = "16111995";
            string requestId = "16111995"; //Guid.NewGuid().ToString();
            string extraData = "";
            transId = "2597830297";


            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;
            MomoUtilities crypto = new MomoUtilities();
            //sign signature SHA256
            string signature = crypto.SignSHA256(rawHash, SecretKey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "transId", transId },
                { "requestType", "refundMoMoWallet" },
                { "signature", signature }

            };
            string responseFromMomo = crypto.SendPaymentRequest(endpoint, message.ToString());
            var momoResponseModel = JsonConvert.DeserializeObject<MomoResponseModel>(responseFromMomo);
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
                };
                success = await this.momoPaymentService.CreateAsync(momoPayments);
            }

            return new AppDomainResult()
            {
                Data = momoResponseModel,
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Test thanh toán vnpay
        /// </summary>
        /// <returns></returns>
        [HttpGet("test-vnpay-payment")]
        public async Task<AppDomainResult> TestVNPayPayment()
        {


            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin thanh toán của momo
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-momo-return-result")]
        public async Task<AppDomainResult> GetMoMoReturnResult([FromQuery] MomoRequestBody momoRequestBody)
        {
            bool success = false;
            this.logger.LogInformation("--------------------------------------------- START GetMoMoNotifyResult");
            if (momoRequestBody != null)
            {
                this.logger.LogInformation("--------------------------------------------- START CHECK GetMoMoNotifyResult");

                // Lấy thông tin đã lưu khi tạo giao dịch momo
                var momoCheckInfos = await this.momoPaymentService.GetAsync(e =>
            e.RequestId == momoRequestBody.requestId
            || e.OrderId == momoRequestBody.orderId
            );
                if (momoCheckInfos != null && momoCheckInfos.Any())
                {
                    var momoCheckInfo = momoCheckInfos.FirstOrDefault();

                    // LẤY RA THÔNG TIN PHIẾU KHÁM
                    var examinationFormInfo = await this.examinationFormService.GetSingleAsync(e => !e.Deleted && e.Id == momoCheckInfo.ExaminationFormId.Value, e => new ExaminationForms()
                    {
                        Id = e.Id,
                        Status = e.Status
                    });

                    // Cập nhật thông tin trạng thái của phiếu khám
                    // NẾU KẾT QUẢ TRẢ VỀ = 0 => CẬP NHẬT LẠI TRẠNG THÁI PHIẾU
                    if (momoRequestBody.resultCode == 0 && momoCheckInfo.ExaminationFormId.HasValue)
                    {
                        if (examinationFormInfo != null)
                        {
                            int status = 0;
                            switch (examinationFormInfo.Status)
                            {
                                case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                                    status = (int)CatalogueUtilities.ExaminationStatus.Confirmed;
                                    break;
                                case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                                    status = (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination;
                                    break;
                                default:
                                    break;
                            }
                            if (status != 0)
                            {
                                UpdateExaminationStatus updateExaminationStatus = new UpdateExaminationStatus()
                                {
                                    ExaminationFormId = momoCheckInfo.ExaminationFormId.Value,
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Status = status,
                                };
                                await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
                            }

                        }

                    }

                    // LƯU LẠI LỊCH SỬ THANH TOÁN CỦA PHIẾU TRONG HỆ THỐNG
                    else
                    {
                        if (examinationFormInfo != null)
                        {
                            int status = (int)CatalogueUtilities.ExaminationStatus.PaymentFailed;
                            if (status != 0)
                            {
                                UpdateExaminationStatus updateExaminationStatus = new UpdateExaminationStatus()
                                {
                                    ExaminationFormId = momoCheckInfo.ExaminationFormId.Value,
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Status = status,
                                };
                                await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
                            }
                        }
                    }
                    // Cập nhật thông tin giao dịch momo
                    momoCheckInfo.ResultCode = momoRequestBody.resultCode;
                    momoCheckInfo.OrderType = momoRequestBody.orderType;
                    momoCheckInfo.Message = momoRequestBody.message;
                    momoCheckInfo.PayType = momoRequestBody.payType;
                    momoCheckInfo.ResponseTime = momoRequestBody.responseTime;
                    momoCheckInfo.TransId = momoRequestBody.transId;
                    momoCheckInfo.Updated = DateTime.Now;
                    momoCheckInfo.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    success = await this.momoPaymentService.UpdateAsync(momoCheckInfo);
                }
                // Nếu không có => lỗi
                else
                {
                    MomoPayments momoPayments = new MomoPayments()
                    {
                        Created = DateTime.Now,
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        Active = true,
                        Deleted = false,
                        Amount = momoRequestBody.amount,
                        RequestId = momoRequestBody.requestId,
                        OrderId = momoRequestBody.orderId,
                        OrderInfo = momoRequestBody.orderInfo,
                        Signature = momoRequestBody.signature,
                        ResultCode = momoRequestBody.resultCode,
                        OrderType = momoRequestBody.orderType,
                        Message = momoRequestBody.message,
                        PayType = momoRequestBody.payType,
                        ResponseTime = momoRequestBody.responseTime,
                        TransId = momoRequestBody.transId,
                    };
                    success = await this.momoPaymentService.CreateAsync(momoPayments);
                }


                this.logger.LogInformation("--------------------------------------------- END CHECK GetMoMoNotifyResult");

            }
            this.logger.LogInformation("--------------------------------------------- END GetMoMoNotifyResult");
            return new AppDomainResult()
            {
                Data = success,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin thanh toán của momo
        /// </summary>
        /// <returns></returns>
        [HttpPost("get-momo-notify-result")]
        [Consumes("application/json")]
        public async Task<AppDomainResult> GetMoMoNotifyResult([FromBody] MomoRequestBody momoRequestBody)
        {
            bool success = false;
            this.logger.LogInformation("--------------------------------------------- START GetMoMoNotifyResult");
            if (momoRequestBody != null)
            {
                this.logger.LogInformation("--------------------------------------------- START CHECK GetMoMoNotifyResult");

                // Lấy thông tin đã lưu khi tạo giao dịch momo
                var momoCheckInfo = await this.momoPaymentService.GetSingleAsync(e =>
            e.RequestId == momoRequestBody.requestId
            || e.OrderId == momoRequestBody.orderId
            );
                if (momoCheckInfo != null)
                {
                    // Cập nhật thông tin trạng thái của phiếu khám
                    if (momoRequestBody.resultCode == 0 && momoCheckInfo.ExaminationFormId.HasValue)
                    {
                        var examinationFormInfos = await this.examinationFormService.GetAsync(e => !e.Deleted && e.Id == momoCheckInfo.ExaminationFormId.Value, e => new ExaminationForms()
                        {
                            Id = e.Id,
                            Status = e.Status
                        });
                        if (examinationFormInfos != null && examinationFormInfos.Any())
                        {
                            var examinationFormInfo = examinationFormInfos.FirstOrDefault();
                            int status = 0;
                            switch (examinationFormInfo.Status)
                            {
                                case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                                    status = (int)CatalogueUtilities.ExaminationStatus.Confirmed;
                                    break;
                                case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                                    status = (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination;
                                    break;
                                default:
                                    break;
                            }
                            if (status != 0)
                            {
                                UpdateExaminationStatus updateExaminationStatus = new UpdateExaminationStatus()
                                {
                                    ExaminationFormId = momoCheckInfo.ExaminationFormId.Value,
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Status = status,
                                };
                                await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
                            }

                        }

                    }

                    // Cập nhật thông tin giao dịch momo
                    momoCheckInfo.ResultCode = momoRequestBody.resultCode;
                    momoCheckInfo.OrderType = momoRequestBody.orderType;
                    momoCheckInfo.Message = momoRequestBody.message;
                    momoCheckInfo.PayType = momoRequestBody.payType;
                    momoCheckInfo.ResponseTime = momoRequestBody.responseTime;
                    momoCheckInfo.TransId = momoRequestBody.transId;
                    momoCheckInfo.Updated = DateTime.Now;
                    momoCheckInfo.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    success = await this.momoPaymentService.UpdateAsync(momoCheckInfo);
                }
                // Nếu không có => lỗi
                else
                {
                    MomoPayments momoPayments = new MomoPayments()
                    {
                        Created = DateTime.Now,
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        Active = true,
                        Deleted = false,
                        Amount = momoRequestBody.amount,
                        RequestId = momoRequestBody.requestId,
                        OrderId = momoRequestBody.orderId,
                        OrderInfo = momoRequestBody.orderInfo,
                        Signature = momoRequestBody.signature,
                        ResultCode = momoRequestBody.resultCode,
                        OrderType = momoRequestBody.orderType,
                        Message = momoRequestBody.message,
                        PayType = momoRequestBody.payType,
                        ResponseTime = momoRequestBody.responseTime,
                        TransId = momoRequestBody.transId,
                    };
                    success = await this.momoPaymentService.CreateAsync(momoPayments);
                }


                this.logger.LogInformation("--------------------------------------------- END CHECK GetMoMoNotifyResult");

            }
            this.logger.LogInformation("--------------------------------------------- END GetMoMoNotifyResult");
            return new AppDomainResult()
            {
                Data = success,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Kiểm tra thông tin thanh toán momo theo requestId hoặc orderId
        /// </summary>
        /// <returns></returns>
        [HttpGet("check-momo-payment/requestId/orderId")]
        public async Task<AppDomainResult> CheckMomopayment([FromQuery] string requestId, string orderId)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            MomoResponseModel momoResponseModel = null;
            if (string.IsNullOrEmpty(requestId) && string.IsNullOrEmpty(orderId))
                throw new AppException("Không có thông tin để check giao dịch");
            var momoPaymentInfos = await this.momoPaymentService.GetAsync(e => !e.Deleted
            && (string.IsNullOrEmpty(requestId) || e.RequestId == requestId)
            && (string.IsNullOrEmpty(orderId) || e.OrderId == orderId)
            );
            if (momoPaymentInfos != null && momoPaymentInfos.Any())
            {
                var momoPaymentInfo = momoPaymentInfos.FirstOrDefault();
                var momoConfigurations = await this.momoConfigurationService.GetAsync(e => !e.Deleted && e.Active);
                if (momoConfigurations != null && momoConfigurations.Any())
                {
                    var momoConfiguration = momoConfigurations.FirstOrDefault();
                    string extraData = string.Empty;
                    //Before sign HMAC SHA256 signature
                    //string rawHash = "partnerCode=" +
                    //    momoConfiguration.PartnerCode + "&accessKey=" +
                    //    momoConfiguration.AccessKey + "&requestId=" +
                    //    momoPaymentInfo.RequestId + "&amount=" +
                    //    momoPaymentInfo.Amount + "&orderId=" +
                    //    momoPaymentInfo.OrderId + "&orderInfo=" +
                    //    momoPaymentInfo.OrderInfo + "&returnUrl=" +
                    //    momoConfiguration.ReturnUrlWebApp + "&notifyUrl=" +
                    //    momoConfiguration.NotifyUrl + "&extraData=" +
                    //    extraData;
                    string rawHash = "accessKey=" +
                        momoConfiguration.AccessKey + "&orderId=" +
                        momoPaymentInfo.OrderId + "&partnerCode=" +
                        momoConfiguration.PartnerCode + "&requestId=" +
                        momoPaymentInfo.RequestId;
                    MomoUtilities crypto = new MomoUtilities();
                    //sign signature SHA256
                    string signature = crypto.SignSHA256(rawHash, SecretKey);

                    //JObject message = new JObject
                    //{
                    //    { "partnerCode", momoConfiguration.PartnerCode },
                    //    { "accessKey", momoConfiguration.AccessKey },
                    //    { "requestId", momoPaymentInfo.RequestId },
                    //    { "amount", momoPaymentInfo.Amount },
                    //    { "orderId", momoPaymentInfo.OrderId },
                    //    { "orderInfo", momoPaymentInfo.OrderInfo },
                    //    { "returnUrl", momoConfiguration.ReturnUrlWebApp },
                    //    { "notifyUrl", momoConfiguration.NotifyUrl },
                    //    { "extraData", extraData },
                    //    { "requestType", "transactionStatus" },
                    //    { "signature", signature }

                    //};

                    JObject message = new JObject
                    {
                        { "partnerCode", momoConfiguration.PartnerCode },
                        { "accessKey", momoConfiguration.AccessKey },
                        { "requestId", momoPaymentInfo.RequestId },
                        { "orderId", momoPaymentInfo.OrderId },
                        { "requestType", "transactionStatus" },
                        { "signature", signature },
                        { "lang", "vi" },

                    };
                    string responseFromMomo = crypto.SendPaymentRequest(endpoint, message.ToString());
                    momoResponseModel = JsonConvert.DeserializeObject<MomoResponseModel>(responseFromMomo);
                }
                else throw new AppException("Không có thông tin để check giao dịch");


            }
            else throw new AppException("Không có thông tin để check giao dịch");

            return new AppDomainResult()
            {
                Data = momoResponseModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }

}
