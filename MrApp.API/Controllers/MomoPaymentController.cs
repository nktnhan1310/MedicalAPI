﻿using AutoMapper;
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
        public const string SecretKey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
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
            string partnerCode = "MOMO5RGX20191128";
            string accessKey = "M8brj9K6E22vXoDB";
            //string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            string orderInfo = "test";
            string returnUrl = "https://s.mrapp.vn/api/momo-payment/get-momo-return-result";
            string notifyurl = "https://s.mrapp.vn/api/momo-payment/get-momo-notify-result";

            string amount = "55000";
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
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
                    Amount = amount,
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
                    // Cập nhật thông tin trạng thái của phiếu khám
                    if (momoRequestBody.errorCode == 0 && momoCheckInfo.ExaminationFormId.HasValue)
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
                    momoCheckInfo.ErrorCode = momoRequestBody.errorCode;
                    momoCheckInfo.LocalMessage = momoRequestBody.localMessage;
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
                        ErrorCode = momoRequestBody.errorCode,
                        LocalMessage = momoRequestBody.localMessage,
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
            //MomoRequestBody momoRequestBody = values.ToObject<MomoRequestBody>();
            //this.logger.LogInformation("--------------------------------------------- START GetMoMoNotifyResult");
            if (momoRequestBody != null)
            {

                MomoPayments momoPayments = new MomoPayments()
                {
                    Created = DateTime.Now,
                    CreatedBy = "system",
                    Active = true,
                    Deleted = false,
                    Amount = momoRequestBody.amount,
                    RequestId = momoRequestBody.requestId,
                    OrderId = momoRequestBody.orderId,
                    OrderInfo = momoRequestBody.orderInfo,
                    Signature = momoRequestBody.signature,
                    ErrorCode = momoRequestBody.errorCode,
                    LocalMessage = momoRequestBody.localMessage,
                    OrderType = momoRequestBody.orderType,
                    Message = momoRequestBody.message,
                    PayType = momoRequestBody.payType,
                    ResponseTime = momoRequestBody.responseTime,
                    TransId = momoRequestBody.transId,
                };
                success = await this.momoPaymentService.CreateAsync(momoPayments);

                this.logger.LogInformation("--------------------------------------------- START CHECK GetMoMoNotifyResult");

                // Lấy thông tin đã lưu khi tạo giao dịch momo
                //    var momoCheckInfos = await this.momoPaymentService.GetAsync(e =>
                //e.RequestId == momoRequestBody.requestId
                //|| e.OrderId == momoRequestBody.orderId
                //);
                //    if (momoCheckInfos != null && momoCheckInfos.Any())
                //    {
                //        var momoCheckInfo = momoCheckInfos.FirstOrDefault();
                //        // Cập nhật thông tin trạng thái của phiếu khám
                //        if (momoRequestBody.errorCode == 0 && momoCheckInfo.ExaminationFormId.HasValue)
                //        {
                //            var examinationFormInfos = await this.examinationFormService.GetAsync(e => !e.Deleted && e.Id == momoCheckInfo.ExaminationFormId.Value, e => new ExaminationForms()
                //            {
                //                Id = e.Id,
                //                Status = e.Status
                //            });
                //            if (examinationFormInfos != null && examinationFormInfos.Any())
                //            {
                //                var examinationFormInfo = examinationFormInfos.FirstOrDefault();
                //                int status = 0;
                //                switch (examinationFormInfo.Status)
                //                {
                //                    case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                //                        status = (int)CatalogueUtilities.ExaminationStatus.Confirmed;
                //                        break;
                //                    case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                //                        status = (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination;
                //                        break;
                //                    default:
                //                        break;
                //                }
                //                if (status != 0)
                //                {
                //                    UpdateExaminationStatus updateExaminationStatus = new UpdateExaminationStatus()
                //                    {
                //                        ExaminationFormId = momoCheckInfo.ExaminationFormId.Value,
                //                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                //                        Status = status,
                //                    };
                //                    await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
                //                }

                //            }



                //        }

                //        // Cập nhật thông tin giao dịch momo
                //        momoCheckInfo.ErrorCode = momoRequestBody.errorCode;
                //        momoCheckInfo.LocalMessage = momoRequestBody.localMessage;
                //        momoCheckInfo.OrderType = momoRequestBody.orderType;
                //        momoCheckInfo.Message = momoRequestBody.message;
                //        momoCheckInfo.PayType = momoRequestBody.payType;
                //        momoCheckInfo.ResponseTime = momoRequestBody.responseTime;
                //        momoCheckInfo.TransId = momoRequestBody.transId;
                //        momoCheckInfo.Updated = DateTime.Now;
                //        momoCheckInfo.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                //        success = await this.momoPaymentService.UpdateAsync(momoCheckInfo);
                //    }
                //    // Nếu không có => lỗi
                //    else
                //    {
                //        MomoPayments momoPayments = new MomoPayments()
                //        {
                //            Created = DateTime.Now,
                //            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                //            Active = true,
                //            Deleted = false,
                //            Amount = momoRequestBody.amount,
                //            RequestId = momoRequestBody.requestId,
                //            OrderId = momoRequestBody.orderId,
                //            OrderInfo = momoRequestBody.orderInfo,
                //            Signature = momoRequestBody.signature,
                //            ErrorCode = momoRequestBody.errorCode,
                //            LocalMessage = momoRequestBody.localMessage,
                //            OrderType = momoRequestBody.orderType,
                //            Message = momoRequestBody.message,
                //            PayType = momoRequestBody.payType,
                //            ResponseTime = momoRequestBody.responseTime,
                //            TransId = momoRequestBody.transId,
                //        };
                //        success = await this.momoPaymentService.CreateAsync(momoPayments);
                //    }


                this.logger.LogInformation("--------------------------------------------- END CHECK GetMoMoNotifyResult");

            }
            else
            {
                MomoPayments momoPayments = new MomoPayments()
                {
                    Created = DateTime.Now,
                    CreatedBy = "system",
                    Active = true,
                    Deleted = false,
                    Amount = string.Empty,
                    RequestId = string.Empty,
                    OrderId = string.Empty,
                    OrderInfo = string.Empty,
                    Signature = string.Empty,
                    ErrorCode = 0,
                    LocalMessage = string.Empty,
                    OrderType = string.Empty,
                    Message = string.Empty,
                    PayType = "asdasdasd",
                    ResponseTime = "asdasdasd",
                    TransId = "asdasdasd",
                };
                success = await this.momoPaymentService.CreateAsync(momoPayments);
            }
            //this.logger.LogInformation("--------------------------------------------- END GetMoMoNotifyResult");
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
                        { "lang", "en" },

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
