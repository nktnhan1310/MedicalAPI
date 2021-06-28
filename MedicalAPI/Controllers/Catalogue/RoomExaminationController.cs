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
using Medical.Utilities;
using System.Net;
using Medical.Extensions;

namespace MedicalAPI.Controllers
{
    [Route("api/room-examination")]
    [ApiController]
    [Description("Phòng khám bệnh")]
    public class RoomExaminationController : CatalogueCoreHospitalController<RoomExaminations, RoomExaminationModel, SearchHopitalExtension>
    {
        private readonly IRoomExaminationService roomExaminationService;
        public RoomExaminationController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<RoomExaminations, RoomExaminationModel, SearchHopitalExtension>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
        }
        
        /// <summary>
        /// Lấy chi tiết lịch khám của phòng
        /// </summary>
        /// <param name="searchHopitalExtension"></param>
        /// <returns></returns>
        [HttpGet("get-room-detail")]
        public async Task<AppDomainResult> GetRoomDetail([FromQuery] SearchHopitalExtension searchHopitalExtension)
        {
            searchHopitalExtension.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            var examinationScheduleDetails = await this.roomExaminationService.GetRoomDetail(searchHopitalExtension);

            return new AppDomainResult()
            {
                Data = mapper.Map<IList<ExaminationScheduleDetailModel>>(examinationScheduleDetails),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
