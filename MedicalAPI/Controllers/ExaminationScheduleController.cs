using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using MedicalAPI.Model;
using MedicalAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Lịch khám")]
    public class ExaminationScheduleController : BaseController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>
    {
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        public ExaminationScheduleController(IServiceProvider serviceProvider, ILogger<BaseController<ExaminationSchedules, ExaminationScheduleModel, SearchExaminationSchedule>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            this.examinationScheduleDetailService = serviceProvider.GetRequiredService<IExaminationScheduleDetailService>();
        }

        /// <summary>
        /// Lấy thông tin chi tiết ca
        /// </summary>
        /// <param name="examinationScheduleId"></param>
        /// <returns></returns>
        [HttpGet("{examinationScheduleId}")]
        public async Task<AppDomainResult> GetExaminatioNScheduleDetails(int examinationScheduleId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var examinationSchedules = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.ScheduleId == examinationScheduleId);
            var examinationScheduleModels = mapper.Map<IList<ExaminationScheduleDetails>>(examinationSchedules);
            appDomainResult.Success = true;
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Data = examinationScheduleModels;

            return appDomainResult;
        }

    }
}
