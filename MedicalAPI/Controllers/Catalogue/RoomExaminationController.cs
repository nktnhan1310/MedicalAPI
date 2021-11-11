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
    public class RoomExaminationController : CatalogueCoreHospitalController<RoomExaminations, RoomExaminationModel, SearchRoomExamination>
    {
        private readonly IRoomExaminationService roomExaminationService;
        private readonly ISpecialListTypeService specialListTypeService;
        public RoomExaminationController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<RoomExaminations, RoomExaminationModel, SearchRoomExamination>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
        }

        /// <summary>
        /// Down load template file import
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-template-import")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public override async Task<ActionResult> DownloadTemplateImport()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, TEMPLATE_FOLDER_NAME, CoreContants.ROOM_EXAMINATION_TEMPLATE_NAME);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            var file = await System.IO.File.ReadAllBytesAsync(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateImport.xlsx");
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RoomExaminationModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Active = true;
                itemModel.Deleted = false;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                SpecialistTypes specialistTypeInfo = null;
                if (itemModel.SpecialistTypeId.HasValue && itemModel.SpecialistTypeId.Value > 0)
                {
                    specialistTypeInfo = await this.specialListTypeService.GetByIdAsync(itemModel.SpecialistTypeId.Value);
                    if (specialistTypeInfo == null) throw new AppException("Thông tin chuyên khoa không tồn tại");
                }
                itemModel.Code = this.roomExaminationService.GenerateRoomCode(itemModel.RoomIndex, itemModel.Name, specialistTypeInfo == null ? string.Empty : specialistTypeInfo.Code);
                var item = mapper.Map<RoomExaminations>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
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
