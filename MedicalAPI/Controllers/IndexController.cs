using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/index")]
    [ApiController]
    [Description("Lấy thông tin stt đóng tiền khám bệnh/thông tin stt khám bệnh")]
    public class IndexController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IExaminationFormService examinationFormService;
        public IndexController(IServiceProvider serviceProvider, IMapper mapper)
        {
            this.mapper = mapper;
            examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
        }


        /// <summary>
        /// Lấy STT hiện tại của dịch vụ
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        [HttpGet("get-current-examination-index")]
        public async Task<AppDomainResult> GetCurrentExaminationIndex([FromQuery] SearchExaminationIndex searchExaminationIndex)
        {
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                searchExaminationIndex.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var indexString = await this.examinationFormService.GetExaminationFormIndex(searchExaminationIndex);
            return new AppDomainResult()
            {
                Success = true,
                Data = indexString
            };
        }
    }
}
