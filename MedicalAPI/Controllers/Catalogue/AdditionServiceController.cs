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
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/addition-service")]
    [ApiController]
    [Description("Dịch vụ phát sinh")]
    [Authorize]
    public class AdditionServiceController : CatalogueCoreHospitalController<AdditionServices, AdditionServiceModel, BaseHospitalSearch>
    {
        private IAdditionServiceDetailService additionServiceDetailService;
        public AdditionServiceController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<AdditionServices, AdditionServiceModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IAdditionServiceType>();
            additionServiceDetailService = serviceProvider.GetRequiredService<IAdditionServiceDetailService>();
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
            if (id <= 0) throw new AppException("Mã không hợp lệ");
            var existItem = await this.catalogueService.GetByIdAsync(id);
            if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
            var itemModel = mapper.Map<AdditionServiceModel>(existItem);
            var additionServiceDetailInfos = await this.additionServiceDetailService.GetAsync(e => !e.Deleted && e.AdditionServiceId == itemModel.Id);
            if (additionServiceDetailInfos != null && additionServiceDetailInfos.Any())
                itemModel.AdditionServiceDetails = mapper.Map<IList<AdditionServiceDetailModel>>(additionServiceDetailInfos);


            return new AppDomainResult()
            {
                Data = itemModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
