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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/medical-record")]
    [ApiController]
    [Description("Hồ sơ bệnh án")]
    [Authorize]
    public class MedicalRecordController : BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>
    {
        private readonly IMedicalRecordAdditionService medicalRecordAdditionService;
        public MedicalRecordController(IServiceProvider serviceProvider, ILogger<BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            medicalRecordAdditionService = serviceProvider.GetRequiredService<IMedicalRecordAdditionService>();
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] MedicalRecordModel itemModel)
        {
            if (LoginContext.Instance.CurrentUser == null || LoginContext.Instance.CurrentUser.UserId != itemModel.UserId)
                throw new UnauthorizedAccessException("Không có quyền truy cập");
            itemModel.UserId = LoginContext.Instance.CurrentUser.UserId;
            return await base.UpdateItem(id, itemModel);
        }

        /// <summary>
        /// Thêm mới hồ sơ bệnh án
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] MedicalRecordModel itemModel)
        {
            if (LoginContext.Instance.CurrentUser != null)
                itemModel.UserId = LoginContext.Instance.CurrentUser.UserId;
            else throw new UnauthorizedAccessException("Không có quyền truy cập");
            return await base.AddItem(itemModel);
        }

        /// <summary>
        /// Lấy thông tin danh sách hồ sơ bệnh án theo user đăng nhập
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] SearchMedicalRecord baseSearch)
        {
            if (LoginContext.Instance.CurrentUser != null)
                baseSearch.UserId = LoginContext.Instance.CurrentUser.UserId;
            else throw new UnauthorizedAccessException("Không có quyền truy cập");
            return await base.GetPagedData(baseSearch);
        }

        /// <summary>
        /// Xóa hồ sơ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            var existMedicalRecords = await this.domainService.GetAsync(e => e.Id == id, e => new MedicalRecords() { UserId = e.UserId });
            if (LoginContext.Instance.CurrentUser == null
                || (existMedicalRecords != null && existMedicalRecords.Any() && LoginContext.Instance.CurrentUser.UserId != existMedicalRecords.FirstOrDefault().UserId))
                throw new UnauthorizedAccessException("Không có quyền truy cập");

            return await base.DeleteItem(id);
        }
    }
}
