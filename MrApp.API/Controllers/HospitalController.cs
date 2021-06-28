using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
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
    [Route("api/hospital")]
    [ApiController]
    [Authorize]
    [Description("Quản lý bệnh viện")]
    public class HospitalController : BaseController
    {
        private readonly IHospitalService hospitalService;
        public HospitalController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
        }

        /// <summary>
        /// Lấy danh sách tất cả bệnh viện
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-hospitals")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetAllHospital()
        {
            var hospitals = await this.hospitalService.GetAsync(e => !e.Deleted && e.Active, e => new Hospitals()
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
            });
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<HospitalModel>>(hospitals),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
