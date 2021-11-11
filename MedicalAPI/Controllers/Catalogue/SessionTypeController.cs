using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using Medical.Utilities;
using Medical.Extensions;

namespace MedicalAPI.Controllers
{
    [Route("api/session-type")]
    [ApiController]
    [Description("Buổi khám bệnh")]
    [Authorize]
    public class SessionTypeController : CatalogueCoreHospitalController<SessionTypes, SessionTypeModel, BaseHospitalSearch>
    {
        public SessionTypeController(IServiceProvider serviceProvider, ILogger<CatalogueCoreHospitalController<SessionTypes, SessionTypeModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISessionTypeService>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] SessionTypeModel itemModel)
        {
            if (!string.IsNullOrEmpty(itemModel.FromTimeDisplayValue))
                itemModel.FromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(itemModel.FromTimeDisplayValue);
            if (!string.IsNullOrEmpty(itemModel.ToTimeDisplayValue))
                itemModel.ToTime = DateTimeUtilities.ConvertTimeToTotalIMinute(itemModel.ToTimeDisplayValue);
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] SessionTypeModel itemModel)
        {
            if (!string.IsNullOrEmpty(itemModel.FromTimeDisplayValue))
                itemModel.FromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(itemModel.FromTimeDisplayValue);
            if (!string.IsNullOrEmpty(itemModel.ToTimeDisplayValue))
                itemModel.ToTime = DateTimeUtilities.ConvertTimeToTotalIMinute(itemModel.ToTimeDisplayValue);
            return await base.UpdateItem(id, itemModel);
        }

    }
}
