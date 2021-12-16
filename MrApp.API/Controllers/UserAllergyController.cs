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
    [Route("api/user-allergy")]
    [ApiController]
    [Description("Quản lý thông tin loại dị ứng")]
    [Authorize]
    public class UserAllergyController : BaseController
    {
        private IUserAllergyService userAllergyService;
        private IUserService userService;
        private IAllergyTypeService allergyTypeService;
<<<<<<< HEAD
        private IAllergyDescriptionTypeService allergyDescriptionTypeService;
        private IUserAllergyDetailService userAllergyDetailService;
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

        public UserAllergyController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            userAllergyService = serviceProvider.GetRequiredService<IUserAllergyService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            allergyTypeService = serviceProvider.GetRequiredService<IAllergyTypeService>();
<<<<<<< HEAD
            allergyDescriptionTypeService = serviceProvider.GetRequiredService<IAllergyDescriptionTypeService>();
            userAllergyDetailService = serviceProvider.GetRequiredService<IUserAllergyDetailService>();
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        /// <summary>
        /// Lấy thông tin danh sách phân trang loại dị ứng
        /// </summary>
        /// <param name="searchUserAllergy"></param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] SearchUserAllergy searchUserAllergy)
        {
            var pagedListModel = new PagedList<UserAllergyModel>();
            searchUserAllergy.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedList = await this.userAllergyService.GetPagedListData(searchUserAllergy);
            if (pagedList != null)
                pagedListModel = mapper.Map<PagedList<UserAllergyModel>>(pagedList);

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = pagedListModel
            };
        }

        /// <summary>
        /// Thêm mới nhóm dị ứng
        /// </summary>
        /// <param name="userAllergyModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> Create([FromBody] UserAllergyModel userAllergyModel)
        {
            bool success = false;
<<<<<<< HEAD
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());


            var itemUpdate = mapper.Map<UserAllergies>(userAllergyModel);
            itemUpdate.Active = true;
            itemUpdate.Created = DateTime.Now;
            itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;

            // KIỂM TRA SỰ TỒN TẠI CỦA DỊ ỨNG NGƯỜI DÙNG
            var errorMessage = await this.userAllergyService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(errorMessage)) throw new AppException(errorMessage);

            success = await this.userAllergyService.CreateAsync(itemUpdate);
            if (!success)
                throw new Exception("Lỗi trong quá trình xử lý");
=======
            if (ModelState.IsValid)
            {
                var itemUpdate = mapper.Map<UserAllergies>(userAllergyModel);
                itemUpdate.Active = true;
                itemUpdate.Created = DateTime.Now;
                itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
                itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                success = await this.userAllergyService.CreateAsync(itemUpdate);
                if (!success)
                    throw new Exception("Lỗi trong quá trình xử lý");
            }
            else throw new AppException(ModelState.GetErrorMessage());
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin dị ứng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userAllergyModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> Update(int id, [FromBody] UserAllergyModel userAllergyModel)
        {
            bool success = false;
            if (ModelState.IsValid)
            {
<<<<<<< HEAD
                var existUserAllergies = await this.userAllergyService.GetAsync(e => !e.Deleted
                && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId
                && e.Id == id
                );
                if (existUserAllergies == null || !existUserAllergies.Any())
=======
                var existUserAllergies = await this.userAllergyService.GetAsync(e => !e.Deleted 
                && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId
                && e.Id == id
                );
                if(existUserAllergies == null || !existUserAllergies.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    throw new Exception("Không tìm thấy thông tin nhóm dị ứng");

                var itemUpdate = mapper.Map<UserAllergies>(userAllergyModel);
                itemUpdate.Id = id;
                itemUpdate.Active = true;
                itemUpdate.Updated = DateTime.Now;
                itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
<<<<<<< HEAD

                // KIỂM TRA SỰ TỒN TẠI CỦA DỊ ỨNG NGƯỜI DÙNG
                var errorMessage = await this.userAllergyService.GetExistItemMessage(itemUpdate);
                if (!string.IsNullOrEmpty(errorMessage)) throw new AppException(errorMessage);

=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                success = await this.userAllergyService.UpdateAsync(itemUpdate);
                if (!success)
                    throw new Exception("Lỗi trong quá trình xử lý");
            }
            else throw new AppException(ModelState.GetErrorMessage());

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin nhóm dị ứng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetById(int id)
        {
            if (id <= 0) throw new AppException("Thông tin id không hợp lệ");
            UserAllergyModel userAllergyModel = null;
<<<<<<< HEAD
            IList<AllergyDescriptionTypeModel> allergyDescriptionTypeModels = new List<AllergyDescriptionTypeModel>();
            var userAllergyInfos = await this.userAllergyService.GetAsync(e => !e.Deleted && e.Active
=======
            var userAllergyInfos = await this.userAllergyService.GetAsync(e => !e.Deleted && e.Active 
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            && e.UserId == LoginContext.Instance.CurrentUser.UserId
            && e.Id == id
            );
            if (userAllergyInfos != null && userAllergyInfos.Any())
            {
                userAllergyModel = mapper.Map<UserAllergyModel>(userAllergyInfos.FirstOrDefault());

                var allergyTypeInfos = await this.allergyTypeService.GetAsync(e => !e.Deleted && e.Active && e.Id == userAllergyModel.AllergyTypeId);
                if (allergyTypeInfos != null && allergyTypeInfos.Any())
                    userAllergyModel.AllergyTypeName = allergyTypeInfos.FirstOrDefault().Name;

                var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Active && e.Id == LoginContext.Instance.CurrentUser.UserId);
                if (userInfos != null && userInfos.Any())
                    userAllergyModel.UserFullName = userInfos.FirstOrDefault().LastName + " " + userInfos.FirstOrDefault().FirstName;
<<<<<<< HEAD

                // Lấy thông tin chi tiết mô tả của user cập nhật
                var userAllergyDetails = await this.userAllergyDetailService.GetAsync(e => !e.Deleted && e.UserAllergyId == userAllergyModel.Id);
                if(userAllergyDetails != null && userAllergyDetails.Any())
                    userAllergyModel.DescriptionTypeIds = userAllergyDetails.Select(e => e.DescriptionTypeId.Value).ToList();

                // Lấy ra tất cả mô tả của thông tin dị ứng
                var allergyDescriptionTypes = await this.allergyDescriptionTypeService.GetAsync(e => !e.Deleted 
                && e.AllergyTypeId == userAllergyModel.AllergyTypeId
                );
                if (allergyDescriptionTypes != null && allergyDescriptionTypes.Any())
                    allergyDescriptionTypeModels = mapper.Map<IList<AllergyDescriptionTypeModel>>(allergyDescriptionTypes);

=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            }
            else throw new AppException("Không tìm thấy thông tin nhóm dị ứng");

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
<<<<<<< HEAD
                Data = new
                {
                    UserAllergyModel = userAllergyModel,
                    AllergyDescriptionTypes = allergyDescriptionTypeModels
                }
=======
                Data = userAllergyModel
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            };
        }

        /// <summary>
        /// Xóa thông tin nhóm dị ứng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete(int id)
        {
            bool success = false;
<<<<<<< HEAD
            var existItems = await this.userAllergyService.GetAsync(e => !e.Deleted && e.Active
=======
            var existItems = await this.userAllergyService.GetAsync(e => !e.Deleted && e.Active 
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            && e.UserId == LoginContext.Instance.CurrentUser.UserId
            && e.Id == id
            );
            if (existItems != null && existItems.Any())
                success = await this.userAllergyService.DeleteAsync(id);
            else throw new AppException("Không tìm thấy thông tin nhóm dị ứng");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa array item
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteItem([FromBody] List<int> itemIds)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (itemIds != null && itemIds.Any())
            {
                foreach (var itemId in itemIds)
                {
                    success = await this.userAllergyService.DeleteAsync(itemId);
                }
            }
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }

    }
}
