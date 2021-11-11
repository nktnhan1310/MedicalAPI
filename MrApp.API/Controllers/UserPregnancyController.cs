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
    [Route("api/user-pregnancy")]
    [ApiController]
    [Description("Quản lý theo dõi thai kỳ")]
    [Authorize]
    public class UserPregnancyController : BaseController
    {
        protected IUserPregnancyService userPregnancyService;
        protected IUserPregnancyDetailService userPregnancyDetailService;
        protected IMedicalRecordService medicalRecordService;

        public UserPregnancyController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            userPregnancyService = serviceProvider.GetRequiredService<IUserPregnancyService>();
            userPregnancyDetailService = serviceProvider.GetRequiredService<IUserPregnancyDetailService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
        }

        /// <summary>
        /// Lấy thông tin danh sách theo dõi thai kỳ
        /// </summary>
        /// <param name="searchUserPregnancy"></param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] SearchUserPregnancy searchUserPregnancy)
        {
            searchUserPregnancy.UserId = LoginContext.Instance.CurrentUser.UserId;
            PagedList<UserPregnancyModel> pagedListModel = new PagedList<UserPregnancyModel>();
            var pagedList = await this.userPregnancyService.GetPagedListData(searchUserPregnancy);
            if (pagedList != null)
                pagedListModel = mapper.Map<PagedList<UserPregnancyModel>>(pagedList);

            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin thai kì
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetById(int id)
        {
            UserPregnancyModel userPregnancyModel = new UserPregnancyModel();
            var userPregnancies = await this.userPregnancyService.GetAsync(e => !e.Deleted && e.Active 
            && e.UserId == LoginContext.Instance.CurrentUser.UserId && e.Id == id);
            if (userPregnancies != null && userPregnancies.Any())
            {
                userPregnancyModel = mapper.Map<UserPregnancyModel>(userPregnancies.FirstOrDefault());
                var userPregnancyDetails = await this.userPregnancyDetailService.GetAsync(e => !e.Deleted && e.Active && e.UserPregnancyId == userPregnancyModel.Id);
                if (userPregnancyDetails != null && userPregnancyDetails.Any())
                    userPregnancyModel.UserPregnancyDetails = mapper.Map<IList<UserPregnancyDetailModel>>(userPregnancyDetails);
            }

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = userPregnancyModel
            };
        }

        /// <summary>
        /// Thêm mới thông tin thai kì
        /// </summary>
        /// <param name="userPregnancyModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddItem([FromBody] UserPregnancyModel userPregnancyModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<UserPregnancies>(userPregnancyModel);
            itemUpdate.Created = DateTime.Now;
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
            itemUpdate.Active = true;
            var medicalRecordInfos = await medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if(medicalRecordInfos != null && medicalRecordInfos.Any())
                itemUpdate.MedicalRecordId = medicalRecordInfos.FirstOrDefault().Id;
            // Kiểm tra thông tin
            string checkExistMessage = await this.userPregnancyService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(checkExistMessage)) throw new AppException(checkExistMessage);

            bool success = await this.userPregnancyService.CreateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin thai kì
        /// </summary>
        /// <param name="userPregnancyModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateItem([FromBody] UserPregnancyModel userPregnancyModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<UserPregnancies>(userPregnancyModel);
            itemUpdate.Updated = DateTime.Now;
            itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
            itemUpdate.Active = true;

            var medicalRecordInfos = await medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (medicalRecordInfos != null && medicalRecordInfos.Any())
                itemUpdate.MedicalRecordId = medicalRecordInfos.FirstOrDefault().Id;
            // Kiểm tra thông tin
            string checkExistMessage = await this.userPregnancyService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(checkExistMessage)) throw new AppException(checkExistMessage);

            bool success = await this.userPregnancyService.UpdateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa thông tin thai kì
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete(int id)
        {
            var existItems = await this.userPregnancyService.GetAsync(e => !e.Deleted && e.Active 
            && e.Id == id && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (existItems == null || !existItems.Any()) throw new AppException("Không tìm thấy thông tin thai kì");
            bool success = await this.userPregnancyService.DeleteAsync(id);
            if (!success) throw new Exception("Xóa thất bại");

            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa thông tin thai kì
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any()) throw new Exception("Không tìm thấy thông tin thai kì");
            var existItems = await this.userPregnancyService.GetAsync(e => !e.Deleted && e.Active 
            && itemIds.Contains(e.Id) && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (existItems == null || !existItems.Any()) throw new AppException("Không tìm thấy thông tin thai kì");
            bool success = true;
            foreach (var itemId in itemIds)
            {
                success &= await this.userPregnancyService.DeleteAsync(itemId);
            }
            if (!success) throw new Exception("Xóa thất bại");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
