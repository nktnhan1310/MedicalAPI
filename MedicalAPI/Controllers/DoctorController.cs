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
    public class DoctorController : BaseController<Doctors, DoctorModel, SearchDoctor>
    {
        private readonly IDoctorDetailService doctorDetailService;
        public DoctorController(IServiceProvider serviceProvider, ILogger<BaseController<Doctors, DoctorModel, SearchDoctor>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDoctorService>();
            this.doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa của bác sĩ
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-details/{doctorId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View, CoreContants.Update })]
        public async Task<AppDomainResult> GetDoctorDetails(int doctorId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.DoctorId == doctorId);
            var doctorDetailModels = mapper.Map<IList<DoctorDetailModel>>(doctorDetails);
            appDomainResult.Success = true;
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Data = doctorDetailModels;
            return appDomainResult;
        }

    }
}
