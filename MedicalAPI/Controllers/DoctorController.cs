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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;

namespace MedicalAPI.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    [Description("Quản lý thông tin bác sĩ")]
    [Authorize]
    public class DoctorController : CoreHospitalController<Doctors, DoctorModel, SearchDoctor>
    {
        private readonly IDoctorDetailService doctorDetailService;
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        private readonly IUserInGroupService userInGroupService;


        public DoctorController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<Doctors, DoctorModel, SearchDoctor>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDoctorService>();
            this.doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
        }

        /// <summary>
        /// Lấy thông tin user chưa được phân bố của bệnh viện
        /// </summary>
        /// <param name="hospitaId"></param>
        /// <returns></returns>
        [HttpGet("get-user-hospital-infos")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInfos(int? hospitaId)
        {
            // Lấy thông tin group bác sĩ
            List<int> groupDoctorIds = new List<int>();
            var doctorGroupRoleInfos = await this.userGroupService.GetAsync(e => (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId) && e.Code == "BS");
            if (doctorGroupRoleInfos != null && doctorGroupRoleInfos.Any())
            {
                groupDoctorIds = doctorGroupRoleInfos.Select(e => e.Id).ToList();
                var userInGroupDoctors = await this.userInGroupService.GetAsync(e => !e.Deleted && groupDoctorIds.Contains(e.UserGroupId));
                if(userInGroupDoctors != null && userInGroupDoctors.Any())
                {
                    var userInGroupDoctorIds = userInGroupDoctors.Select(e => e.UserId).Distinct().ToList();
                    // Lấy tất cả user đã được phân bố cho bệnh viện
                    var doctorUserIds = this.domainService.Get(e => !e.Deleted
                    && e.UserId.HasValue
                    && (!hospitaId.HasValue || e.HospitalId == hospitaId)
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value)
                    ).Select(e => e.UserId).ToList();

                    var userInfos = await this.userService.GetAsync(e => !e.Deleted
                    && e.HospitalId.HasValue
                    && !e.IsAdmin
                    && userInGroupDoctorIds.Contains(e.Id)
                    && e.Id != LoginContext.Instance.CurrentUser.UserId
                    && (!hospitaId.HasValue || e.HospitalId == hospitaId)
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value)
                    && !doctorUserIds.Contains(e.Id)
                    , e => new Users()
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Phone = e.Phone,
                        Email = e.Email,
                        HospitalId = e.HospitalId
                    });
                    return new AppDomainResult()
                    {
                        Data = mapper.Map<IList<UserModel>>(userInfos),
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new AppException("Không có tài khoản bác sĩ thích hợp");
            }
            else throw new AppException("Không có tài khoản bác sĩ thích hợp");
        }

        /// <summary>
        /// Lấy thông tin theo bác sĩ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {

            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);

            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<DoctorModel>(item);
                    var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.DoctorId == id);
                    var doctorDetailModels = mapper.Map<IList<DoctorDetailModel>>(doctorDetails);
                    itemModel.DoctorDetails = doctorDetailModels;
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
    }
}
