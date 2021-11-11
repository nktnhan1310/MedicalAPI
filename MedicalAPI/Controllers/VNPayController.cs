using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private IConfiguration configuration;
        private IHttpContextAccessor httpContextAccessor;
        private IVNPayPaymentHistoryService vNPayPaymentHistoryService;
        private IPaymentHistoryService paymentHistoryService;
        private IExaminationFormService examinationFormService;
        private IMedicalRecordService medicalRecordService;
        private ILogger<VNPayController> logger;

        public VNPayController(IServiceProvider serviceProvider, IMapper mapper, ILogger<VNPayController> logger
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.vNPayPaymentHistoryService = serviceProvider.GetRequiredService<IVNPayPaymentHistoryService>();
            this.paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
            this.examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
            this.medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();

            this.logger = logger;
        }

        [HttpGet("test-vnpay-payment")]
        public async Task<AppDomainResult> TestVNPayPayment()
        {
            bool success = true;

            string vnp_Returnurl = configuration.GetSection("MySettings:vnp_Returnurl").Value.ToString();
            string vnp_Url = configuration.GetSection("MySettings:vnp_Url").Value.ToString(); //URL thanh toan cua VNPAY 
            string vnp_TmnCode = configuration.GetSection("MySettings:vnp_TmnCode").Value.ToString(); //Ma website
            string vnp_HashSecret = configuration.GetSection("MySettings:vnp_HashSecret").Value.ToString(); //Chuoi bi mat
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                throw new AppException("Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong appsetting");
            long orderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            long amount = 100000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            string status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            string orderDescription = "test";
            DateTime createdDate = DateTime.Now;
            VNPayUtilities vnpay = new VNPayUtilities();
            vnpay.AddRequestData("vnp_Version", VNPayUtilities.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "NCB");
            vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VNPayUtilities.GetIpAddress(httpContextAccessor));
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", createdDate.AddMinutes(30).ToString("yyyyMMddHHmmss"));
            ////Billing
            //vnpay.AddRequestData("vnp_Bill_Mobile", "0123456789");
            //vnpay.AddRequestData("vnp_Bill_Email", "vnpaytest@vnpay.vn");
            //string fullName = "NGUYEN VAN A";
            //if (!string.IsNullOrEmpty(fullName))
            //{
            //    var indexof = fullName.IndexOf(' ');
            //    vnpay.AddRequestData("vnp_Bill_FirstName", fullName.Substring(0, indexof));
            //    vnpay.AddRequestData("vnp_Bill_LastName", fullName.Substring(indexof + 1, fullName.Length - indexof - 1));
            //}
            //vnpay.AddRequestData("vnp_Bill_Address", "Phòng 315, Công ty VNPAY, Tòa nhà TĐL, 22 Láng Hạ, Đống Đa, Hà Nội");
            //vnpay.AddRequestData("vnp_Bill_City", "Hà Nội");
            //vnpay.AddRequestData("vnp_Bill_Country", "VN");
            //vnpay.AddRequestData("vnp_Bill_State", "");

            //// Invoice

            //vnpay.AddRequestData("vnp_Inv_Phone", "02437764668");
            //vnpay.AddRequestData("vnp_Inv_Email", "vnpaytest@vnpay.vn");
            //vnpay.AddRequestData("vnp_Inv_Customer", "Nguyen Van A");
            //vnpay.AddRequestData("vnp_Inv_Address", "22 Láng Hạ, Phường Láng Hạ, Quận Đống Đa, TP Hà Nội");
            //vnpay.AddRequestData("vnp_Inv_Company", "Công ty Cổ phần giải pháp Thanh toán Việt Nam");
            //vnpay.AddRequestData("vnp_Inv_Taxcode", "0102182292");
            //vnpay.AddRequestData("vnp_Inv_Type", "I");
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return new AppDomainResult()
            {
                Data = new
                {
                    paymentUrl = paymentUrl,
                    orderId = orderId,
                    createdDate = createdDate.ToString("yyyyMMddHHmmss")
                },
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };

        }

        /// <summary>
        /// Kiểm tra hoàn tiền giao dịch ở vnpay
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="transactionNo"></param>
        /// <param name="transacionDate"></param>
        /// <returns></returns>
        [HttpGet("test-vnpay-refund/orderId")]
        public async Task<AppDomainResult> TestVNPayRefund(string orderId, string transactionNo, string transacionDate)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            string resultHTML = string.Empty;

            string vnpayApiUrl = configuration.GetSection("MySettings:querydr").Value.ToString();
            string vnpTmnCode = configuration.GetSection("MySettings:vnp_TmnCode").Value.ToString(); //Ma website
            string vnpHashSecret = configuration.GetSection("MySettings:vnp_HashSecret").Value.ToString(); //Chuoi bi mat

            var vnpay = new VNPayUtilities();
            var createDate = DateTime.Now;

            //var objectRefund = new VNPayRefundModel
            //{
            //    vnp_RequestId = Guid.NewGuid().ToString(),
            //    vnp_Version = VNPayUtilities.VERSION,
            //    vnp_Command = "refund",
            //    vnp_TmnCode = vnpTmnCode,
            //    vnp_TransactionType = "02",
            //    vnp_TxnRef = orderId,
            //    vnp_Amount = 100000 * 100,
            //    vnp_OrderInfo = "Khach hang tra lai hang",
            //    vnp_TransactionNo = transactionNo,
            //    vnp_TransactionDate = transacionDate,
            //    vnp_CreateBy = "nhannkt1995@gmail.com",
            //    vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
            //    vnp_IpAddr = VNPayUtilities.GetIpAddress(httpContextAccessor),
            //    vnp_SecureHash = string.Empty
            //};
            //string data = vnpHashSecret + objectRefund.vnp_RequestId + "|" + objectRefund.vnp_Version + "|" + objectRefund.vnp_Command + "|" + objectRefund.vnp_TmnCode + "|" + objectRefund.vnp_TransactionType + "|" + objectRefund.vnp_TxnRef + "|" + objectRefund.vnp_Amount + "|" + "|" + objectRefund.vnp_TransactionNo + "|" + objectRefund.vnp_TransactionDate + "|" + objectRefund.vnp_CreateBy + "|" + objectRefund.vnp_CreateDate + "|" + objectRefund.vnp_IpAddr + "|" + objectRefund.vnp_OrderInfo;
            //objectRefund.vnp_SecureHash = VNPayUtilities.HmacSHA512(vnpHashSecret, data);

            //var refundObject = JsonConvert.SerializeObject(objectRefund);
            //var buffer = System.Text.Encoding.UTF8.GetBytes(refundObject);
            //var byteContent = new ByteArrayContent(buffer);
            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            ////          ServicePointManager.ServerCertificateValidationCallback =
            ////delegate (object sender, X509Certificate certificate, X509Chain chain,
            ////    SslPolicyErrors sslPolicyErrors)
            ////{ return true; };

            //var refundResponseModel = new VNPayRefundResponseModel();

            //using (var client = new HttpClient())
            //{
            //    try
            //    {
            //        HttpResponseMessage response = await client.PostAsync(vnpayApiUrl, byteContent);
            //        if (response.IsSuccessStatusCode)
            //        {
            //            var responseJson = await response.Content.ReadAsStringAsync();

            //            return new AppDomainResult()
            //            {
            //                Success = false,
            //                ResultCode = (int)HttpStatusCode.OK,
            //                Data = responseJson
            //            };

            //            //if (!string.IsNullOrEmpty(responseJson))
            //            //{
            //            //    refundResponseModel = JsonConvert.DeserializeObject<VNPayRefundResponseModel>(responseJson);
            //            //}
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        return new AppDomainResult()
            //        {
            //            Success = false,
            //            ResultCode = (int)HttpStatusCode.BadRequest,
            //            Messages = new List<string>() { ex.Message },
            //        };
            //    }

            //}

            var amountrf = 100000 * 100;
            vnpay.AddRequestData("vnp_Version", VNPayUtilities.VERSION);
            vnpay.AddRequestData("vnp_Command", "refund");
            vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);

            vnpay.AddRequestData("vnp_TransactionType", "02");
            vnpay.AddRequestData("vnp_TxnRef", orderId);

            vnpay.AddRequestData("vnp_Amount", amountrf.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", "REFUND ORDERID:" + orderId);
            vnpay.AddRequestData("vnp_TransDate", transacionDate);
            vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CreateBy", "nhannkt1995@gmail.com");

            vnpay.AddRequestData("vnp_IpAddr", VNPayUtilities.GetIpAddress(httpContextAccessor));

            var strDatax = "";

            var refundtUrl = vnpay.CreateRequestUrl(vnpayApiUrl, vnpHashSecret);

            //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var request = (HttpWebRequest)WebRequest.Create(refundtUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        strDatax = reader.ReadToEnd();
                    }
                }
                    
            resultHTML = "<b>VNPAY RESPONSE:</b> " + strDatax;

            return new AppDomainResult()
            {
                Data = resultHTML,//refundResponseModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Kiểm tra kết quả giao dịch vnpay
        /// </summary>
        /// <param name="vNPaymentQueryModel"></param>
        /// <returns></returns>
        [HttpGet("get-vnpay-result")]
        public async Task<ActionResult> GetVNPayResult([FromQuery] VNPaymentRequestModel vNPaymentQueryModel)
        {
            string rspCode = string.Empty;
            string message = string.Empty;
            string vnp_HashSecret = configuration.GetSection("MySettings:vnp_HashSecret").Value.ToString(); //Chuoi bi mat
            VNPayUtilities vNPay = new VNPayUtilities();
            bool checkSignature = vNPay.ValidateSignature(vNPaymentQueryModel.vnp_SecureHash, vnp_HashSecret);
            // Nếu chữ kí OK
            // => Tiến hành cập nhật trạng thái giao dịch
            if (checkSignature)
            {
                // LẤY THÔNG TIN THANH TOÁN ĐÃ GỬI ĐI
                var vnPaymentHistgory = await this.vNPayPaymentHistoryService.GetSingleAsync(e => e.OrderId == vNPaymentQueryModel.vnp_TxnRef);
                // Không tìm thấy đơn thanh toán
                if (vnPaymentHistgory == null) return new JsonResult(new { RspCode = "97", Message = "Không tìm thấy đơn thanh toán" });

                if (vnPaymentHistgory.Amount != vNPaymentQueryModel.vnp_Amount) return new JsonResult(new { RspCode = "04", Message = "Không tìm thấy đơn thanh toán" });
                if (vnPaymentHistgory.Status != "0") return new JsonResult(new { RspCode = "04", Message = "Không tìm thấy đơn thanh toán" });

            }
            else
            {
                rspCode = "97";
                message = "Invalid signature";
            }
            return new JsonResult(new { RspCode = rspCode, Message = message });
        }

        /// <summary>
        /// Cập nhật thông tin kết quả giao dịch vnpay
        /// </summary>
        /// <param name="vNPaymentQueryModel"></param>
        /// <returns></returns>
        [HttpGet("update-vnpay-result")]
        public async Task<AppDomainResult> UpdateVNPayResult([FromQuery] VNPaymentRequestModel vNPaymentQueryModel)
        {
            if (vNPaymentQueryModel == null) throw new AppException("Không tìm thấy thông tin query");
            AppDomainResult appDomainResult = new AppDomainResult();
            string rspCode = string.Empty;
            string message = string.Empty;
            string status = "0";
            string vnp_HashSecret = configuration.GetSection("MySettings:vnp_HashSecret").Value.ToString(); //Chuoi bi mat
            VNPayUtilities vNPay = this.AddVNPayResponseData(vNPaymentQueryModel);
            bool checkSignature = vNPay.ValidateSignature(vNPaymentQueryModel.vnp_SecureHash, vnp_HashSecret);
            // Nếu chữ kí OK
            // => Tiến hành cập nhật trạng thái giao dịch
            if (!checkSignature)
            {
                await this.UpdateVNPayPaymenHistory(null, vNPaymentQueryModel, status, "97", "Invalid signature");
                return new AppDomainResult() { Data = new { RspCode = "97", Message = "Invalid signature" } };
            }

            // LẤY THÔNG TIN THANH TOÁN ĐÃ GỬI ĐI
            var vnPaymentHistory = await this.vNPayPaymentHistoryService.GetSingleAsync(e => e.OrderId == vNPaymentQueryModel.vnp_TxnRef);
            // Không tìm thấy đơn thanh toán
            if (vnPaymentHistory == null)
            {
                await this.UpdateVNPayPaymenHistory(null, vNPaymentQueryModel, status, "01", "Order not found");
                return new AppDomainResult() { Data = new { RspCode = "01", Message = "Order not found" } };
            }
            // LẤY RA THÔNG TIN PHIẾU TỪ LỊCH SỬ THANH TOÁN CỦA VNPAY
            var examinationForm = await this.examinationFormService.GetSingleAsync(e => e.Id == vnPaymentHistory.ExaminationFormId);
            if (examinationForm == null)
            {
                await this.UpdateVNPayPaymenHistory(vnPaymentHistory, vNPaymentQueryModel, status, "01", "Order not found");
                return new AppDomainResult() { Data = new { RspCode = "01", Message = "Order not found" } };
            }

            if ((vnPaymentHistory.Amount * 100) != vNPaymentQueryModel.vnp_Amount)
            {
                await this.UpdateVNPayPaymenHistory(vnPaymentHistory, vNPaymentQueryModel, status, "04", "invalid amount");
                return new AppDomainResult() { Data = new { RspCode = "04", Message = "invalid amount" } };
            }
            if (vnPaymentHistory.Status != "0")
            {
                await this.UpdateVNPayPaymenHistory(vnPaymentHistory, vNPaymentQueryModel, status, "02", "Order already confirmed");
                return new AppDomainResult() { Data = new { RspCode = "02", Message = "Order already confirmed" } };
            }

            // Thanh toán thất bại (LỖI TRONG QUÁ TRÌNH XỬ LÝ)
            UpdateExaminationStatus updateExaminationStatus = null;
            if (vNPaymentQueryModel.vnp_ResponseCode != "00" || vNPaymentQueryModel.vnp_TransactionStatus != "00")
            {
                updateExaminationStatus = new UpdateExaminationStatus()
                {
                    CreatedBy = "System",
                    Status = (int)CatalogueUtilities.ExaminationStatus.PaymentFailed,
                    ExaminationFormId = vnPaymentHistory.ExaminationFormId.Value
                };
                status = "2";
                this.logger.LogInformation(string.Format("VNPay thanh toán thất bại, orderid: {0}, vnpaytranid: {1}, ResponseCode: {2}", vNPaymentQueryModel.vnp_TxnRef, vNPaymentQueryModel.vnp_TransactionNo, vNPaymentQueryModel.vnp_ResponseCode));
            }
            // Thanh toán thành công
            else
            {
                updateExaminationStatus = new UpdateExaminationStatus()
                {
                    CreatedBy = "System",
                    Status = (int)CatalogueUtilities.ExaminationStatus.Confirmed,
                    ExaminationFormId = vnPaymentHistory.ExaminationFormId.Value
                };
                this.logger.LogInformation(string.Format("VNPay thanh toán thành công, orderid: {0}, vnpaytranid: {1}", vNPaymentQueryModel.vnp_TxnRef, vNPaymentQueryModel.vnp_TransactionNo));
                status = "1";
            }
            // CẬP NHẬT LẠI THÔNG TIN PHIẾU KHÁM
            if (updateExaminationStatus != null)
            {
                await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
            }
            await this.UpdateVNPayPaymenHistory(vnPaymentHistory, vNPaymentQueryModel, status, vNPaymentQueryModel.vnp_ResponseCode, "Confirm Success");

            return new AppDomainResult()
            {
                Data = new
                {
                    RspCode = "00",
                    Message = "Confirm Success"
                }
            };
        }

        private VNPayUtilities AddVNPayResponseData(VNPaymentRequestModel vnPaymentRequestModel)
        {
            VNPayUtilities vNPay = new VNPayUtilities();
            vNPay.AddResponseData("vnp_Amount", vnPaymentRequestModel.vnp_Amount.ToString());
            vNPay.AddResponseData("vnp_BankCode", vnPaymentRequestModel.vnp_BankCode);
            vNPay.AddResponseData("vnp_BankTranNo", vnPaymentRequestModel.vnp_BankTranNo);
            vNPay.AddResponseData("vnp_CardType", vnPaymentRequestModel.vnp_CardType);
            vNPay.AddResponseData("vnp_OrderInfo", vnPaymentRequestModel.vnp_OrderInfo);
            vNPay.AddResponseData("vnp_PayDate", vnPaymentRequestModel.vnp_PayDate);
            vNPay.AddResponseData("vnp_ResponseCode", vnPaymentRequestModel.vnp_ResponseCode);
            vNPay.AddResponseData("vnp_TmnCode", vnPaymentRequestModel.vnp_TmnCode);
            vNPay.AddResponseData("vnp_TransactionNo", vnPaymentRequestModel.vnp_TransactionNo.ToString());
            vNPay.AddResponseData("vnp_TransactionStatus", vnPaymentRequestModel.vnp_TransactionStatus);
            vNPay.AddResponseData("vnp_TxnRef", vnPaymentRequestModel.vnp_TxnRef.ToString());
            if (!string.IsNullOrEmpty(vnPaymentRequestModel.vnp_SecureHashType))
                vNPay.AddResponseData("vnp_SecureHashType", vnPaymentRequestModel.vnp_SecureHashType);
            vNPay.AddResponseData("vnp_SecureHash", vnPaymentRequestModel.vnp_SecureHash);
            return vNPay;
        }

        /// <summary>
        /// Cập nhật lại Lịch sử thanh toán qua vnpay
        /// </summary>
        /// <param name="vNPayPaymentHistory"></param>
        /// <param name="vnPaymentRequestModel"></param>
        /// <param name="status"></param>
        /// <param name="resultCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task UpdateVNPayPaymenHistory(VNPayPaymentHistories vNPayPaymentHistory, VNPaymentRequestModel vnPaymentRequestModel
            , string status
            , string resultCode, string message)
        {
            // Thêm mới lịch sử thanh toán qua vnpay
            if (vNPayPaymentHistory == null)
            {
                int? userId = null;
                int? examinationFormId = null;
                long amount = 0;
                if (vnPaymentRequestModel != null)
                {
                    // Lấy thông tin lịch sử thanh toán thông qua mã đơn thanh toán
                    var vnPayPaymentHistory = await this.vNPayPaymentHistoryService.GetSingleAsync(e => !e.Deleted && e.OrderId == vnPaymentRequestModel.vnp_TxnRef);
                    if (vnPayPaymentHistory != null)
                    {
                        // LẤY THÔNG TIN PHIẾU THANH TOÁN
                        var examinationFormInfo = await this.examinationFormService.GetSingleAsync(e => e.Id == vnPayPaymentHistory.ExaminationFormId);
                        if (examinationFormInfo != null)
                        {
                            amount = Convert.ToInt64(examinationFormInfo.Price ?? 0);
                            // LẤY THÔNG TIN HỒ SƠ NGƯỜI BỆNH
                            var medicalRecordInfo = await this.medicalRecordService.GetSingleAsync(e => e.Id == examinationFormInfo.RecordId);
                            if (medicalRecordInfo != null) userId = medicalRecordInfo.UserId;
                        }
                    }
                }
                VNPayPaymentHistories vNPayPaymentHistories = new VNPayPaymentHistories()
                {
                    Created = DateTime.Now,
                    CreatedBy = "System",
                    Active = true,
                    Deleted = false,
                    OrderId = vnPaymentRequestModel.vnp_TxnRef,
                    ExaminationFormId = examinationFormId,
                    UserId = userId,
                    Status = status,
                    PayStatus = resultCode,
                    Message = message,
                    Amount = amount,
                };
                await this.vNPayPaymentHistoryService.CreateAsync(vNPayPaymentHistories);
            }
            // CẬP NHẬT LẠI THÔNG TIN THANH TOÁN CHO THÔNG TIN THANH TOÁN CỦA VNPAY
            else
            {
                vNPayPaymentHistory.Status = status;
                vNPayPaymentHistory.PayStatus = resultCode;
                vNPayPaymentHistory.Message = message;
                vNPayPaymentHistory.Updated = DateTime.Now;
                vNPayPaymentHistory.UpdatedBy = "System";
                Expression<Func<VNPayPaymentHistories, object>>[] includeProperties = new Expression<Func<VNPayPaymentHistories, object>>[]
                {
                    e => e.Updated,
                    e => e.UpdatedBy,
                    e => e.Status,
                    e => e.PayStatus,
                    e => e.Message
                };
                await this.vNPayPaymentHistoryService.UpdateFieldAsync(vNPayPaymentHistory, includeProperties);
            }
        }
    }
}
