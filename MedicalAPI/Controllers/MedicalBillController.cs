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
    [Route("api/medical-bills")]
    [ApiController]
    [Description("Quản lý đơn thuốc")]
    [Authorize]
    public class MedicalBillController : CatalogueCoreHospitalController<MedicalBills, MedicalBillModel, SearchMedicalBill>
    {
        private readonly IMedicalBillService medicalBillService;
        private readonly IMedicineService medicineService;
        private readonly IMomoPaymentService momoPaymentService;
        private readonly IMomoConfigurationService momoConfigurationService;
        private readonly IPaymentMethodService paymentMethodService;

        public MedicalBillController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<MedicalBills, MedicalBillModel, SearchMedicalBill>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IMedicalBillService>();
            medicalBillService = serviceProvider.GetRequiredService<IMedicalBillService>();
            medicineService = serviceProvider.GetRequiredService<IMedicineService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            SearchMedicalBill baseSearch = new SearchMedicalBill();
            IList<MedicalBillModel> itemModels = new List<MedicalBillModel>();
            baseSearch.PageIndex = 1;
            baseSearch.PageSize = int.MaxValue;
            baseSearch.OrderBy = "Id desc";
            baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            baseSearch.MedicalBillId = id;
            var pagedItems = await this.catalogueService.GetPagedListData(baseSearch);
            //var item = await this.catalogueService.GetByIdAsync(id);
            if (pagedItems != null && pagedItems.Items.Any())
            {
                var item = pagedItems.Items.FirstOrDefault();
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<MedicalBillModel>(item);
                    var medicines = await this.medicineService.GetAsync(e => !e.Deleted && e.MedicalBillId == id);
                    if (medicines != null && medicines.Any())
                        itemModel.Medicines = mapper.Map<IList<MedicineModel>>(medicines);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
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
            if (updateMedicalBillStatus.PaymentMethodId.HasValue)
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
