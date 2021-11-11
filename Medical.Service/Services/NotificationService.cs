using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class NotificationService : CoreHospitalService<Notifications, SearchNotification>, INotificationService
    {
        public NotificationService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Notification_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchNotification baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@FromUserId", baseSearch.FromUserId),
                new SqlParameter("@ToUserId", baseSearch.ToUserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@NotificationTypeId", baseSearch.NotificationTypeId),
                new SqlParameter("@NotificationId", baseSearch.NotificationId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Notifications item)
        {
            bool result = false;
            if (item != null)
            {
                item.Id = 0;
                await this.unitOfWork.Repository<Notifications>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();

                // ---------------- Tạo thông tin báo cáo cho user nếu active notification
                if (item.Active && !item.IsSendNotify)
                    await CreateNotifyUserData(item);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Notifications item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<Notifications>(item);
                unitOfWork.Repository<Notifications>().Update(exists);
            }
            await unitOfWork.SaveAsync();
            result = true;
            if (exists.Active && !exists.IsSendNotify)
                await CreateNotifyUserData(item);
            return result;
        }

        /// <summary>
        /// Tạo thông tin thông báo cho user theo điều kiện tương ứng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateNotifyUserData(Notifications item)
        {
            await Task.Run(() =>
            {
                object obj = new object();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    if (item.UserIds == null)
                        item.UserIds = new List<int>();
                    if (item.HospitalIds == null)
                        item.HospitalIds = new List<int>();
                    if (item.UserGroupIds == null)
                        item.UserGroupIds = new List<int>();
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NotificationId", item.Id),
                        new SqlParameter("@FromUserId", item.FromUserId),
                        new SqlParameter("@UserIds", string.Join(",", item.UserIds)),
                        new SqlParameter("@HospitalId", item.HospitalId),
                        new SqlParameter("@HospitalIds", string.Join(",", item.HospitalIds)),
                        new SqlParameter("@UserGroupIds", string.Join(",", item.UserGroupIds)),
                        new SqlParameter("@CreatedBy", item.CreatedBy)
                    };

                    connection = (SqlConnection)medicalDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Nofification_CreateData";
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        /// <summary>
        /// Xóa thông tin user thông báo
        /// </summary>
        /// <param name="notificationIds"></param>
        /// <param name="userId"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserNotifications(List<int> notificationIds, int userId, int? hospitalId)
        {
            bool success = false;
            foreach (var notificationId in notificationIds)
            {
                var notificationUsers = await this.unitOfWork.Repository<NotificationApplicationUser>().GetQueryable().Where(e => !e.Deleted
            && e.ToUserId == userId
            && e.NotificationId == notificationId
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            ).ToListAsync();
                if (notificationUsers != null && notificationUsers.Any())
                {
                    foreach (var notificationUser in notificationUsers)
                    {
                        this.unitOfWork.Repository<NotificationApplicationUser>().Delete(notificationUser);
                    }
                    await this.unitOfWork.SaveAsync();
                    success = true;
                }
            }
            return success;
        }


        /// <summary>
        /// Tạo custom notification trong hệ thống
        /// </summary>
        /// <param name="fromUserId">Gửi từ user</param>
        /// <param name="hospitalId">Mã bệnh viện</param>
        /// <param name="toUserIds">List user nhận</param>
        /// <param name="webUrl">Link web URL</param>
        /// <param name="appUrl">Link App Url</param>
        /// <param name="createdBy">Được tạo bởi</param>
        /// <param name="notificationTypeCode">Loại thông báo</param>
        /// <param name="defaultTemplateCode">Mã code mẫu template mặc định</param>
        /// <returns></returns>
        public async Task<bool> CreateCustomNotificationUser(
            int? fromUserId // Được gửi từ user
            , int? hospitalId // Mã bệnh viện
            , List<int> toUserIds // Gửi đến user nào
            , string webUrl // Link web
            , string appUrl // Link app
            , string createdBy // Được tạo bởi
            , int? examinationFormid // Mã phiếu hẹn
            , bool isMrApp = false
            , string notificationTypeCode = "USER" // Loại thông báo
            , string defaultTemplateCode = "" //Template mặc định
            , List<int> examinationformIds = null // Danh sách phiếu
            , List<int> examinationScheduleIds = null // Danh sách lịch trực
            , List<int> examinationScheduleDetailIds = null // Danh sách ca trực
            )
        {
            bool success = false;
            int? notificationTypeId = null;

            var notificationTypeUserInfo = await this.unitOfWork.Repository<NotificationTypes>().GetQueryable().Where(e => e.Code == CatalogueUtilities.NotificationType.USER.ToString()).FirstOrDefaultAsync();
            if (notificationTypeUserInfo != null) notificationTypeId = notificationTypeUserInfo.Id;

            // Lấy thông tin mẫu default template theo mặc định
            var notifyTemplateInfo = await this.unitOfWork.Repository<NotificationTemplates>().GetQueryable().Where(e => e.Code == defaultTemplateCode).FirstOrDefaultAsync();

            if (toUserIds != null && toUserIds.Any())
            {
                if (notifyTemplateInfo != null)
                {
                    Notifications notifications = new Notifications()
                    {
                        Active = true,
                        Deleted = false,
                        Created = DateTime.Now,
                        CreatedBy = createdBy,
                        AppUrl = appUrl,
                        WebUrl = webUrl,
                        FromUserId = fromUserId,
                        HospitalId = hospitalId,
                        IsRead = false,
                        IsSendNotify = false,
                        NotificationTemplateId = notifyTemplateInfo.Id,
                        ExaminationFormIds = examinationformIds != null ? string.Join(";", examinationformIds) : string.Empty,
                    };
                    await this.unitOfWork.Repository<Notifications>().CreateAsync(notifications);
                    await this.unitOfWork.SaveAsync();
                    switch (defaultTemplateCode)
                    {
                        // 1.Khởi tạo thông tin bác sĩ bác sĩ
                        case CoreContants.NOTI_TEMPLATE_DOCTOR_CREATE:
                            {
                                foreach (var toUserId in toUserIds)
                                {

                                    NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                    {
                                        Active = true,
                                        Deleted = false,
                                        IsRead = false,
                                        Created = DateTime.Now,
                                        CreatedBy = createdBy,
                                        HospitalId = hospitalId,
                                        NotificationContent = notifyTemplateInfo.Content,
                                        NotificationId = notifications.Id,
                                        ToUserId = toUserId,
                                    };
                                    await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                }
                                await this.unitOfWork.SaveAsync();
                            }
                            break;
                        // 2.Thông báo hồ sơ bệnh án cho user (Nếu admin tạo)
                        case CoreContants.NOTI_TEMPLATE_MEDICAL_RECORD_CREATE:
                            {
                                foreach (var toUserId in toUserIds)
                                {
                                    NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                    {
                                        Active = true,
                                        Deleted = false,
                                        IsRead = false,
                                        Created = DateTime.Now,
                                        CreatedBy = createdBy,
                                        HospitalId = hospitalId,
                                        NotificationContent = notifyTemplateInfo.Content,
                                        NotificationId = notifications.Id,
                                        ToUserId = toUserId,
                                    };
                                    await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                }
                                await this.unitOfWork.SaveAsync();
                            }
                            break;
                        // 3.Tạo phiếu khám bệnh - lịch hẹn (Examination Form)
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_CREATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();

                                if (examinationFormInfo != null)
                                {

                                    // 3.1: Admin tạo => thông báo cho user, doctor
                                    if (!isMrApp)
                                    {
                                        // Tạo thông báo cho user
                                        foreach (var toUserId in toUserIds)
                                        {
                                            NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                            {
                                                Active = true,
                                                Deleted = false,
                                                IsRead = false,
                                                Created = DateTime.Now,
                                                CreatedBy = createdBy,
                                                HospitalId = hospitalId,
                                                NotificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")),
                                                NotificationId = notifications.Id,
                                                ToUserId = toUserId,
                                            };
                                            await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                        }
                                        await this.unitOfWork.SaveAsync();
                                    }

                                }
                            }
                            break;
                        // THÔNG BÁO TEMPLATE TẠO THÔNG BÁO CHO BÁC SĨ
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_DOCTOR_CREATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();
                                if (examinationFormInfo != null)
                                {
                                    // Lấy thông tin bác sĩ để gửi thông báo
                                    var doctorInfo = await this.unitOfWork.Repository<Doctors>().GetQueryable().Where(e => e.Id == examinationFormInfo.DoctorId).FirstOrDefaultAsync();
                                    // Tạo thông báo cho bác sĩ (nếu có)
                                    if (doctorInfo != null && doctorInfo.UserId.HasValue)
                                    {
                                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                        {
                                            Active = true,
                                            Deleted = false,
                                            IsRead = false,
                                            Created = DateTime.Now,
                                            CreatedBy = createdBy,
                                            HospitalId = hospitalId,
                                            NotificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")),
                                            NotificationId = notifications.Id,
                                            ToUserId = doctorInfo.UserId.Value,
                                        };
                                        await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }

                            }
                            break;
                        // 4. Cập nhật thông tin phiếu khám (Examination form info)
                        // 4.1: Bác sĩ cập nhật thông tin phiếu khám (Chờ tái khám) => thông báo cho user
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_DOCTOR_UPDATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();
                                if (examinationFormInfo != null)
                                {
                                    // Tạo thông báo cho user
                                    foreach (var toUserId in toUserIds)
                                    {
                                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                        {
                                            Active = true,
                                            Deleted = false,
                                            IsRead = false,
                                            Created = DateTime.Now,
                                            CreatedBy = createdBy,
                                            HospitalId = hospitalId,
                                            NotificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ReExaminationDate.HasValue ? examinationFormInfo.ReExaminationDate.Value.ToString("dd/MM/yyyy") : string.Empty),
                                            NotificationId = notifications.Id,
                                            ToUserId = toUserId,
                                        };
                                        await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }
                            }
                            break;
                        // 4.2: Xác nhận thông tin tái khám => thông báo cho doctor
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_USER_UPDATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();
                                if (examinationFormInfo != null)
                                {
                                    // Lấy thông tin bác sĩ để gửi thông báo
                                    var doctorInfo = await this.unitOfWork.Repository<Doctors>().GetQueryable().Where(e => e.Id == examinationFormInfo.DoctorId).FirstOrDefaultAsync();
                                    // Tạo thông báo cho bác sĩ (nếu có)
                                    if (doctorInfo != null && doctorInfo.UserId.HasValue)
                                    {
                                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                        {
                                            Active = true,
                                            Deleted = false,
                                            IsRead = false,
                                            Created = DateTime.Now,
                                            CreatedBy = createdBy,
                                            HospitalId = hospitalId,
                                            NotificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")),
                                            NotificationId = notifications.Id,
                                            ToUserId = doctorInfo.UserId.Value,
                                        };
                                        await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }
                            }
                            break;
                        // 4.3: Admin cập nhật thông tin phiếu thành Đã xác nhận/Đã xác nhận tái khám => thông báo cho user + bác sĩ
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_ADMIN_UPDATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();
                                if (examinationFormInfo != null)
                                {
                                    // Tạo thông báo cho user
                                    foreach (var toUserId in toUserIds)
                                    {
                                        // Lấy thông tin bác sĩ để gửi thông báo
                                        var doctorInfo = await this.unitOfWork.Repository<Doctors>().GetQueryable().Where(e => e.Id == examinationFormInfo.DoctorId).FirstOrDefaultAsync();

                                        string notificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")
                                            , doctorInfo != null ? "Bác sĩ " + (doctorInfo.LastName + " " + doctorInfo.LastName) : string.Empty
                                            );
                                        var paymentMethodInfo = await this.unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => e.Id == examinationFormInfo.PaymentMethodId).FirstOrDefaultAsync();


                                        if (!string.IsNullOrEmpty(examinationFormInfo.ExaminationIndex)
                                            && examinationFormInfo.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                            && (paymentMethodInfo == null || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString()
                                            || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.TRANSFER.ToString()
                                            )
                                            )
                                        {
                                            notificationContent += string.Format(". STT khám: {0}", examinationFormInfo.ExaminationIndex);
                                        }
                                        if (!string.IsNullOrEmpty(examinationFormInfo.ExaminationPaymentIndex)
                                            && examinationFormInfo.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                            && (paymentMethodInfo == null || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.MOMO.ToString() || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.APP.ToString())
                                            )
                                            notificationContent += string.Format(". STT thanh toán: {0}", examinationFormInfo.ExaminationPaymentIndex);

                                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                        {
                                            Active = true,
                                            Deleted = false,
                                            IsRead = false,
                                            Created = DateTime.Now,
                                            CreatedBy = createdBy,
                                            HospitalId = hospitalId,
                                            NotificationContent = notificationContent,
                                            NotificationId = notifications.Id,
                                            ToUserId = toUserId,
                                        };
                                        await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }
                            }
                            break;
                        // 4.4: Thanh toán MOMO/Online => Thông báo cho user + doctor
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_PAYMENT_UPDATE:
                            {
                                var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable().Where(e => e.Id == examinationFormid).FirstOrDefaultAsync();
                                if (examinationFormInfo != null)
                                {
                                    // Tạo thông báo cho user
                                    foreach (var toUserId in toUserIds)
                                    {
                                        // Lấy thông tin bác sĩ để gửi thông báo
                                        var doctorInfo = await this.unitOfWork.Repository<Doctors>().GetQueryable().Where(e => e.Id == examinationFormInfo.DoctorId).FirstOrDefaultAsync();

                                        string notificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")
                                            , doctorInfo != null ? "Bác sĩ " + (doctorInfo.LastName + " " + doctorInfo.LastName) : string.Empty
                                            );
                                        var paymentMethodInfo = await this.unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => e.Id == examinationFormInfo.PaymentMethodId).FirstOrDefaultAsync();

                                        if (!string.IsNullOrEmpty(examinationFormInfo.ExaminationPaymentIndex)
                                            && examinationFormInfo.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                            && (paymentMethodInfo == null || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.MOMO.ToString() || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.APP.ToString())
                                            )
                                            notificationContent += string.Format(". STT thanh toán: {0}", examinationFormInfo.ExaminationPaymentIndex);

                                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                        {
                                            Active = true,
                                            Deleted = false,
                                            IsRead = false,
                                            Created = DateTime.Now,
                                            CreatedBy = createdBy,
                                            HospitalId = hospitalId,
                                            NotificationContent = notificationContent,
                                            NotificationId = notifications.Id,
                                            ToUserId = toUserId,
                                        };
                                        await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }
                            }
                            break;
                        // 5. THÔNG BÁO HỦY LỊCH TRỰC CỦA BÁC SĨ => Thông báo cho user + doctor
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_SCHEDULE_DT_DELETE:
                            {

                            }
                            break;
                        // 6. THÔNG BÁO HỦY CHI TIẾT CA TRỰC CHO BÁC SĨ => Thông báo cho user + doctor
                        case CoreContants.TEMPLATE_EXAMINATION_SCHEDULE_DETAIL_DT_DELETE:
                            {
                                if (examinationformIds != null && examinationformIds.Any())
                                {
                                    var examinationFormInfos = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                        .Where(e => !e.Deleted && examinationformIds.Contains(e.Id)).ToListAsync();
                                    if (examinationFormInfos != null && examinationFormInfos.Any())
                                    {
                                        foreach (var examinationFormInfo in examinationFormInfos)
                                        {
                                            // Gửi thông báo cho bác sĩ
                                            var doctorExaminationInfo = await this.unitOfWork.Repository<Doctors>()
                                                .GetQueryable().Where(e => !e.Deleted && e.Id == examinationFormInfo.DoctorId).FirstOrDefaultAsync();

                                            var examinationScheduleDetailInfo = await this.unitOfWork.Repository<ExaminationScheduleDetails>()
                                                .GetQueryable().Where(e => !e.Deleted && e.Id == examinationFormInfo.ExaminationScheduleDetailId).FirstOrDefaultAsync();

                                            if (doctorExaminationInfo != null)
                                            {
                                                // Tạo thông báo cho bác sĩ
                                                NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                                {
                                                    Active = true,
                                                    Deleted = false,
                                                    IsRead = false,
                                                    Created = DateTime.Now,
                                                    CreatedBy = createdBy,
                                                    HospitalId = hospitalId,
                                                    NotificationContent = string.Format(notifyTemplateInfo.Content, examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy"), examinationScheduleDetailInfo.FromTimeText + " - " + examinationScheduleDetailInfo.ToTimeText),
                                                    NotificationId = notifications.Id,
                                                    ToUserId = doctorExaminationInfo.UserId,
                                                };
                                                await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                            }
                                            // Gửi thông báo cho khách
                                            var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable()
                                                .Where(e => !e.Deleted && e.Id == examinationFormInfo.RecordId).FirstOrDefaultAsync();
                                            if (medicalRecordInfo != null)
                                            {
                                                // Tạo thông báo cho user
                                                NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                                {
                                                    Active = true,
                                                    Deleted = false,
                                                    IsRead = false,
                                                    Created = DateTime.Now,
                                                    CreatedBy = createdBy,
                                                    HospitalId = hospitalId,
                                                    NotificationContent = string.Format("Lịch khám ngày {0} với ca khám {1} đã bị hủy. Quý khách vui lòng chờ hoàn tiền nếu đã thanh toán", examinationFormInfo.ExaminationDate.ToString("dd/MM/yyyy"), examinationScheduleDetailInfo.FromTimeText + " - " + examinationScheduleDetailInfo.ToTimeText),
                                                    NotificationId = notifications.Id,
                                                    ToUserId = medicalRecordInfo.UserId,
                                                };
                                                await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                            }

                                        }
                                    }

                                    // Lưu thông tin thông báo cho user
                                    await this.unitOfWork.SaveAsync();
                                }
                            }
                            break;
                        // 7. THÔNG BÁO THỜI GIAN KHÁM TIẾP THEO CỦA USER
                        case CoreContants.TEMPLATE_NEXT_EXAMINATION_NOTIFY:
                            {
                                if (examinationformIds != null && examinationformIds.Any())
                                {
                                    var examinationFormInfos = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                        .Where(e => examinationformIds.Contains(e.Id)).ToListAsync();

                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else throw new AppException("Không tìm thấy mẫu template");
            }
            else throw new AppException("Không có thông tin user gửi đi");

            return success;
        }


        #region CRON JOB

        /// <summary>
        /// CRON JOB CLEAR BỚT DỮ LIỆU THÔNG BÁO CỦA USER SAU 7 NGÀY
        /// </summary>
        /// <returns></returns>
        public async Task ClearNotificationDataJob()
        {
            await Task.Run(() =>
            {
                object obj = new object();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)medicalDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Nofification_ClearData";
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        #endregion


    }
}
