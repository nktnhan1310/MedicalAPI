using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace MedicalAPI.Controllers
{
    [Route("api/examination-schedule")]
    [ApiController]
    [Description("Lịch khám")]
    [Authorize]
    public class ExaminationScheduleController : CoreHospitalController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>
    {
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        private readonly IDoctorService doctorService;
        private readonly ISpecialListTypeService specialListTypeService;
        private readonly IDoctorDetailService doctorDetailService;
        private readonly ISessionTypeService sessionTypeService;
        private readonly IConfigTimeExaminationService configTimeExaminationService;
        private readonly IExaminationScheduleService examinationScheduleService;
        private readonly IExaminationScheduleMappingUserService examinationScheduleMappingUserService;

        public ExaminationScheduleController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            this.examinationScheduleDetailService = serviceProvider.GetRequiredService<IExaminationScheduleDetailService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
            doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            sessionTypeService = serviceProvider.GetRequiredService<ISessionTypeService>();
            configTimeExaminationService = serviceProvider.GetRequiredService<IConfigTimeExaminationService>();
            examinationScheduleService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            examinationScheduleMappingUserService = serviceProvider.GetRequiredService<IExaminationScheduleMappingUserService>();
        }

        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }

            SearchExaminationSchedule searchExaminationSchedule = new SearchExaminationSchedule()
            {
                PageIndex = 1,
                PageSize = 1,
                OrderBy = "Id desc",
                HospitalId = LoginContext.Instance.CurrentUser.HospitalId,
                ExaminationScheduleId = id
            };
            var pagedItems = await this.domainService.GetPagedListData(searchExaminationSchedule);

            //var item = await this.domainService.GetByIdAsync(id);

            if (pagedItems != null && pagedItems.Items.Any())
            {
                var item = pagedItems.Items.FirstOrDefault();

                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<ExaminationScheduleModel>(item);
                    var examinationScheduleDetails = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.ImportScheduleId == item.ImportScheduleId);
                    if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                        itemModel.ExaminationScheduleDetails = mapper.Map<IList<ExaminationScheduleDetailModel>>(examinationScheduleDetails);
                    var examinationScheduleMappingUsers = await this.examinationScheduleMappingUserService.GetAsync(e => e.ExaminationScheduleId == itemModel.Id);
                    if (examinationScheduleMappingUsers != null && examinationScheduleMappingUsers.Any())
                        itemModel.NurseIds = examinationScheduleMappingUsers.Select(e => e.DoctorId).ToList();

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
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] ExaminationScheduleModel itemModel)
        {
            if (itemModel != null && itemModel.ExaminationScheduleDetails != null && itemModel.ExaminationScheduleDetails.Any())
            {
                foreach (var examinationScheduleDetail in itemModel.ExaminationScheduleDetails)
                {
                    if (!string.IsNullOrEmpty(examinationScheduleDetail.FromTimeDisplay))
                    {
                        examinationScheduleDetail.FromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.FromTimeDisplay);
                        if (examinationScheduleDetail.FromTime.HasValue && examinationScheduleDetail.FromTime.Value > 0)
                            examinationScheduleDetail.FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.FromTime.Value);
                    }

                    if (!string.IsNullOrEmpty(examinationScheduleDetail.ToTimeDisplay))
                    {
                        examinationScheduleDetail.ToTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.ToTimeDisplay);
                        if (examinationScheduleDetail.ToTime.HasValue && examinationScheduleDetail.ToTime.Value > 0)
                            examinationScheduleDetail.ToTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.ToTime.Value);
                    }

                }
            }
            return await base.AddItem(itemModel);
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] ExaminationScheduleModel itemModel)
        {
            if (itemModel != null && itemModel.ExaminationScheduleDetails != null && itemModel.ExaminationScheduleDetails.Any())
            {
                foreach (var examinationScheduleDetail in itemModel.ExaminationScheduleDetails)
                {
                    if (!string.IsNullOrEmpty(examinationScheduleDetail.FromTimeDisplay))
                    {
                        examinationScheduleDetail.FromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.FromTimeDisplay);
                        if (examinationScheduleDetail.FromTime.HasValue && examinationScheduleDetail.FromTime.Value > 0)
                            examinationScheduleDetail.FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.FromTime.Value);
                    }

                    if (!string.IsNullOrEmpty(examinationScheduleDetail.ToTimeDisplay))
                    {
                        examinationScheduleDetail.ToTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.ToTimeDisplay);
                        if (examinationScheduleDetail.ToTime.HasValue && examinationScheduleDetail.ToTime.Value > 0)
                            examinationScheduleDetail.ToTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.ToTime.Value);
                    }

                }
            }
            return await base.UpdateItem(id, itemModel);
        }

        /// <summary>
        /// Thêm nhanh lịch trực
        /// </summary>
        /// <param name="examinationScheduleExtensionModel"></param>
        /// <returns></returns>
        [HttpPost("add-multiple-schedule")]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddMultipleSchedule([FromBody] ExaminationScheduleExtensionModel examinationScheduleExtensionModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
<<<<<<< HEAD
=======
            bool success = true;

>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            if (examinationScheduleExtensionModel.ExaminationScheduleDetails != null && examinationScheduleExtensionModel.ExaminationScheduleDetails.Any())
            {
                foreach (var examinationScheduleDetail in examinationScheduleExtensionModel.ExaminationScheduleDetails)
                {
                    if (!string.IsNullOrEmpty(examinationScheduleDetail.FromTimeDisplay))
                    {
                        examinationScheduleDetail.FromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.FromTimeDisplay);
                        if (examinationScheduleDetail.FromTime.HasValue && examinationScheduleDetail.FromTime.Value > 0)
                            examinationScheduleDetail.FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.FromTime.Value);
                    }

                    if (!string.IsNullOrEmpty(examinationScheduleDetail.ToTimeDisplay))
                    {
                        examinationScheduleDetail.ToTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetail.ToTimeDisplay);
                        if (examinationScheduleDetail.ToTime.HasValue && examinationScheduleDetail.ToTime.Value > 0)
                            examinationScheduleDetail.ToTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(examinationScheduleDetail.ToTime.Value);
                    }
                }
            }
            var itemUpdate = mapper.Map<ExaminationScheduleExtensions>(examinationScheduleExtensionModel);
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                itemUpdate.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;

            if (!itemUpdate.HospitalId.HasValue || itemUpdate.HospitalId.Value <= 0) throw new AppException("Chưa chọn thông tin bệnh viện");

<<<<<<< HEAD
            var successTask = this.examinationScheduleService.AddMultipleExaminationSchedule(new List<ExaminationScheduleExtensions>() { itemUpdate }, itemUpdate.HospitalId ?? 0);
            return new AppDomainResult()
            {
                Success = await successTask,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa tất cả ca trực theo danh sách phòng được chọn
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-room-examination-schedule")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteRoomExaminationShedule([FromBody] List<int> roomIds)
        {
            if (roomIds == null) throw new AppException("Không tìm thấy thông tin phòng hợp lệ");
            bool success = await this.examinationScheduleService.DeleteRoomExaminationSchedule(roomIds);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
=======
            

            success = await this.examinationScheduleService.AddMultipleExaminationSchedule(new List<ExaminationScheduleExtensions>() { itemUpdate }, itemUpdate.HospitalId ?? 0);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
<<<<<<< HEAD
        /// LẤY DANH SÁCH PHÒNG KHÁM THEO THÔNG TIN CA TRỰC
        /// </summary>
        /// <param name="searchRoomExaminationSchedule"></param>
        /// <returns></returns>
        [HttpGet("get-room-examination-schedule")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetRoomExaminationScheduleDetail([FromQuery] SearchRoomExaminationSchedule searchRoomExaminationSchedule)
        {
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                searchRoomExaminationSchedule.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var pagedList = await this.examinationScheduleDetailService.GetRoomExaminationScheduleDetailInfo(searchRoomExaminationSchedule);
            var pagedListModel = mapper.Map<PagedList<ExaminationScheduleDetailModel>>(pagedList);

            return new AppDomainResult()
            {
                Data = pagedListModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách khoảng thời gian của chi tiết ca trực
        /// </summary>
        /// <param name="searchExaminationScheduleDetail"></param>
        /// <returns></returns>
        [HttpGet("get-paging-examination-schedule-detail")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetExaminationScheduleDetailPaging([FromQuery] SearchExaminationScheduleDetail searchExaminationScheduleDetail)
        {
            if (searchExaminationScheduleDetail == null) throw new AppException("Thông tin filter không tồn tại");
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                searchExaminationScheduleDetail.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var pagedList = this.examinationScheduleDetailService.GetPagedListData(searchExaminationScheduleDetail);
            var pagedListModel = mapper.Map<PagedList<ExaminationScheduleDetailModel>>(await pagedList);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = pagedListModel
            };
        }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Down load template file import
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-template-import")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public async Task<ActionResult> DownloadTemplateImport()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, CoreContants.TEMPLATE_FOLDER_NAME, CoreContants.EXAMINATION_SCHEDULE_TEMPLATE_NAME);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            var file = await System.IO.File.ReadAllBytesAsync(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateImport.xlsx");
        }

        /// <summary>
        /// Tải về file kết quả sau khi import
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("download-import-result-file/{fileName}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public async Task<ActionResult> DownloadImportFileResult(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name không tồn tại!");
            if (env == null)
                throw new Exception("IHostingEnvironment is null => inject to constructor");
            var webRoot = env.ContentRootPath;
            string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
            var file = await System.IO.File.ReadAllBytesAsync(path);
            // Xóa file thư mục temp
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("KetQuaCD-{0:dd-MM-yyyy_HH_mm_ss}{1}", DateTime.Now, Path.GetExtension(fileName)));
        }

        /// <summary>
        /// Import file danh mục
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("import-template-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.Import })]
        public async Task<AppDomainImportResult> ImportTemplateFile(int? hospitalId, IFormFile file)
        {
            AppDomainImportResult appDomainImportResult = new AppDomainImportResult();
            var fileStream = file.OpenReadStream();
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            if (!hospitalId.HasValue || hospitalId.Value <= 0) throw new AppException("Không tìm thấy thông tin bệnh viện");
<<<<<<< HEAD
            appDomainImportResult = await this.examinationScheduleService.ImportExaminationSchedule(fileStream, hospitalId.Value);
=======
            appDomainImportResult = await this.examinationScheduleService.ImportExaminationSchedule(fileStream, LoginContext.Instance.CurrentUser.UserName, hospitalId.Value);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            if (appDomainImportResult.ResultFile != null)
            {
                var webRoot = env.ContentRootPath;
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
                FileUtils.CreateDirectory(Path.Combine(webRoot, TEMP_FOLDER_NAME));
                FileUtils.SaveToPath(path, appDomainImportResult.ResultFile);
                appDomainImportResult.ResultFile = null;
                appDomainImportResult.DownloadFileName = fileName;
            }
            return appDomainImportResult;
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa theo bệnh viện được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-by-hospital/{hospitalId}")]
        public async Task<AppDomainResult> GetSpecialistTypeByHospital(int hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId);
            var specialistTypeModels = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = specialistTypeModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-by-hospital/{hospitalId}")]
        public async Task<AppDomainResult> GetDoctorByHospital(int hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var doctorByHospitals = await this.doctorService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId);
            var doctorModels = mapper.Map<IList<DoctorModel>>(doctorByHospitals);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = doctorModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa của bác sĩ được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-by-doctor/hospitalId/{doctorId}")]
        public async Task<AppDomainResult> GetSpecialistTypeByDoctor(int? hospitalId, int doctorId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            List<int> specialistTypeIds = new List<int>();
            var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.Active && e.DoctorId == doctorId);
            if (doctorDetails != null && doctorDetails.Any())
                specialistTypeIds = doctorDetails.Select(e => e.SpecialistTypeId).ToList();
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            && specialistTypeIds.Contains(e.Id));
            var specialistTypeModels = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = specialistTypeModels
            };

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin bác sĩ theo chuyên khoa được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="specialistTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-by-specialist-type/hospitalId/{specialistTypeId}")]
        public async Task<AppDomainResult> GetDoctorBySpecialistType(int? hospitalId, int specialistTypeId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            List<int> doctorIds = new List<int>();
            var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.Active && e.SpecialistTypeId == specialistTypeId);
            if (doctorDetails != null && doctorDetails.Any())
                doctorIds = doctorDetails.Select(e => e.DoctorId).ToList();
            if (doctorIds.Any())
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
                var doctorBySpecialistTypes = await this.doctorService.GetAsync(e => !e.Deleted && e.Active
                && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
                && doctorIds.Contains(e.Id));
                var doctorModels = mapper.Map<IList<DoctorModel>>(doctorBySpecialistTypes);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = doctorModels
                };
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách bác sĩ thay thế cho lịch hiện tại
        /// </summary>
        /// <param name="examinationScheduleId"></param>
        /// <returns></returns>
        [HttpGet("get-replace-doctor/{examinationScheduleId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetReplaceDoctor(int examinationScheduleId)
        {
            IList<DoctorModel> doctors = new List<DoctorModel>();
            var examinationScheduleInfo = await this.domainService.GetByIdAsync(examinationScheduleId);
            if (examinationScheduleInfo != null)
            {

                var doctorInfos = await this.doctorService.GetAsync(e => !e.Deleted && e.Active
                   && e.HospitalId == examinationScheduleInfo.HospitalId
                   && e.Id != examinationScheduleInfo.DoctorId
                   );
                doctors = mapper.Map<IList<DoctorModel>>(doctorInfos);
            }

            return new AppDomainResult
            {
                Success = true,
                Data = doctors,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #region Examination Schedule Details

        /// <summary>
        /// Lấy danh sách buổi khám
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-session-type")]
        public async Task<AppDomainResult> GetSessionType()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var sessionTypes = await this.sessionTypeService.GetAsync(e => !e.Deleted && e.Active);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<SessionTypeModel>>(sessionTypes)
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin cấu hình ca khám theo buổi khám
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="sesstionTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination/hospitalId/sessionTypeId")]
        public async Task<AppDomainResult> GetConfigTimeExamination(int? hospitalId, int? sesstionTypeId)
        {
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            AppDomainResult appDomainResult = new AppDomainResult();
            var configTimeExaminations = await this.configTimeExaminationService.GetAsync(e => !e.Deleted
            && e.Active
            && (!sesstionTypeId.HasValue || e.SessionId == sesstionTypeId)
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            );
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<ConfigTimeExamniationModel>>(configTimeExaminations)
            };

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin chi tiết ca
        /// </summary>
        /// <param name="examinationScheduleId"></param>
        /// <returns></returns>
        [HttpGet("get-examination-schedule-details/{examinationScheduleId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetExaminationScheduleDetails(int examinationScheduleId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var examinationSchedules = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.ScheduleId == examinationScheduleId);
            var examinationScheduleModels = mapper.Map<IList<ExaminationScheduleDetails>>(examinationSchedules);
            appDomainResult.Success = true;
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Data = examinationScheduleModels;
            return appDomainResult;
        }

        #endregion



    }
}
