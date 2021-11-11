using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/user-vaccine-process")]
    [ApiController]
    [Description("Quản lý quy trình chích ngừa của user")]
    [Authorize]
    public class UserVaccineProcessController : BaseController
    {
        private readonly IUserVaccineProcessService userVaccineProcessService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IUserService userService;
        private readonly IVaccineTypeService vaccineTypeService;
        private readonly IMedicalRecordDetailService medicalRecordDetailService;
        public UserVaccineProcessController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            userVaccineProcessService = serviceProvider.GetRequiredService<IUserVaccineProcessService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            vaccineTypeService = serviceProvider.GetRequiredService<IVaccineTypeService>();
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
        }

        /// <summary>
        /// Lấy thông tin danh sách vaccine user đã chích/chưa chích theo nhất ký của mẹ/bé
        /// </summary>
        /// <param name="targetId">Đối tượng</param>
        /// <returns></returns>
        [HttpGet("get-user-injected-vaccine/targetId")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserVaccineList(int? targetId)
        {
            if (!targetId.HasValue) throw new AppException("Vui lòng chọn đối tượng!");

            // Danh sách kết quả vaccine chưa chích/đã chích
            List<UserVaccineTypeModel> userVaccineTypeModels = new List<UserVaccineTypeModel>();
            // Danh sách vaccine theo đối tượng
            IList<VaccineTypes> vaccineTypeByTargets = new List<VaccineTypes>();
            var targetId_s = targetId.Value.ToString();
            // Lấy thông tin hồ sơ của người bệnh
            var medicalRecordInfo = await this.medicalRecordService.GetSingleAsync(e => !e.Deleted && e.UserId == LoginContext.Instance.CurrentUser.UserId);

            // Lấy ra thông tin vaccine phải tiêm cho đối tượng hiện tại
            vaccineTypeByTargets = await this.vaccineTypeService.GetAsync(e => !e.Deleted && e.TargetIdValues.Contains(targetId_s));
            if (vaccineTypeByTargets.Any())
            {
                var vaccineTypeByTargetIds = vaccineTypeByTargets.Select(e => e.Id).ToList();
                var medicalRecordDetails = await this.medicalRecordDetailService.GetAsync(e => !e.Deleted
                && e.VaccineTypeId.HasValue && e.VaccineTypeId.Value > 0
                && vaccineTypeByTargetIds.Contains(e.VaccineTypeId.Value)
                );

                foreach (var vaccineTypeByTarget in vaccineTypeByTargets)
                {
                    var finishInjectVaccine = medicalRecordDetails.Where(e => e.VaccineTypeId == vaccineTypeByTarget.Id).OrderByDescending(e => e.ExaminationDate).FirstOrDefault();

                    userVaccineTypeModels.Add(new UserVaccineTypeModel()
                    {
                        VaccineTypeId = vaccineTypeByTarget.Id,
                        VaccineTypeName = vaccineTypeByTarget.Name,
                        InjectDate = finishInjectVaccine != null ? finishInjectVaccine.ExaminationDate : null,
                        Status = finishInjectVaccine != null ? 1 : 0,
                    });
                }
            }

            // Lấy ra danh sách vaccine user đã tiêm trước đó.
            var userVaccineProcesses = await this.userVaccineProcessService.GetAsync(e => !e.Deleted && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (userVaccineProcesses != null && userVaccineProcesses.Any())
            {

                foreach (var userVaccineProcess in userVaccineProcesses)
                {
                    var vaccineInfo = await this.vaccineTypeService.GetSingleAsync(e => e.Id == userVaccineProcess.VaccineTypeId, e => new VaccineTypes { Name = e.Name });

                    userVaccineTypeModels.Add(new UserVaccineTypeModel()
                    {
                        VaccineTypeId = userVaccineProcess.VaccineTypeId,
                        VaccineTypeName = vaccineInfo.Name,
                        InjectDate = userVaccineProcess.InjectDate,
                        Status = 1,
                    });
                }
            }

            return new AppDomainResult()
            {
                Success = true,
                Data = userVaccineTypeModels,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách quy trình chích của user
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] SearchUserVaccineProcess baseSearch)
        {
            var pagedListModel = new PagedList<UserVaccineProcessModel>();
            var pagedList = await this.userVaccineProcessService.GetPagedListData(baseSearch);
            pagedListModel = mapper.Map<PagedList<UserVaccineProcessModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin quy trình khám theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetById(int id)
        {
            UserVaccineProcessModel userVaccineProcessModel = null;
            var userVaccineProcess = await this.userVaccineProcessService.GetSingleAsync(e => !e.Deleted && e.Id == id
            && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (userVaccineProcess != null)
            {
                userVaccineProcessModel = mapper.Map<UserVaccineProcessModel>(userVaccineProcess);
                // Lấy thông tin hồ sơ của user
                var medicalRecordInfo = await this.medicalRecordService.GetSingleAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (medicalRecordInfo != null) userVaccineProcessModel.UserFullName = medicalRecordInfo.UserFullName;
            }

            return new AppDomainResult()
            {
                Success = true,
                Data = userVaccineProcessModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới quy trình chích ngừa của user
        /// </summary>
        /// <param name="userVaccineProcessModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddNew([FromBody] UserVaccineProcessModel userVaccineProcessModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<UserVaccineProcesses>(userVaccineProcessModel);
            itemUpdate.Created = DateTime.Now;
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.Active = true;
            itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
            var medicalRecordInfo = await this.medicalRecordService.GetSingleAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (medicalRecordInfo != null) itemUpdate.MedicalRecordId = medicalRecordInfo.Id;

            bool success = await this.userVaccineProcessService.CreateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật quy trình chích ngừa của user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userVaccineProcessModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> Update(int id, [FromBody] UserVaccineProcessModel userVaccineProcessModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            if (id <= 0) throw new AppException("Id item không hợp lệ");
            var existItem = await this.userVaccineProcessService.GetSingleAsync(e => e.Id == id && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (existItem == null) throw new AppException("Không tim thấy thông tin item");
            var itemUpdate = mapper.Map<UserVaccineProcesses>(userVaccineProcessModel);
            itemUpdate.Id = id;
            itemUpdate.Updated = DateTime.Now;
            itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.Active = true;
            itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
            // Lấy thông tin hồ sơ của bệnh nhân
            var medicalRecordInfo = await this.medicalRecordService.GetSingleAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (medicalRecordInfo != null) itemUpdate.MedicalRecordId = medicalRecordInfo.Id;

            bool success = await this.userVaccineProcessService.UpdateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa thông tin quy trình khám của user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete(int id)
        {
            if (id <= 0) throw new AppException("Id item không hợp lệ");
            var existItem = await this.userVaccineProcessService.GetSingleAsync(e => e.Id == id && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (existItem == null) throw new AppException("Không tim thấy thông tin item");
            bool success = await this.userVaccineProcessService.DeleteAsync(id);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa thông tin quy trình khám của user
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("delete-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete([FromBody] List<int> ids)
        {
            if (ids != null && ids.Any()) throw new AppException("Danh sách item id không hợp lệ");
            var existItems = await this.userVaccineProcessService.GetAsync(e => ids.Contains(e.Id)
            && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (existItems == null || !existItems.Any()) throw new AppException("Không tim thấy thông tin danh sách item xóa");
            bool success = true;
            foreach (var id in ids)
            {
                success &= await this.userVaccineProcessService.DeleteAsync(id);
            }
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
