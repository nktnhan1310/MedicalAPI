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
using Medical.Extensions;

namespace MedicalAPI.Controllers
{
    [Route("api/specialist-type")]
    [ApiController]
    [Description("Chuyên khoa khám bệnh")]
    public class SpecialistTypeController : CatalogueCoreHospitalController<SpecialistTypes, SpecialistTypeModel, SearchSpecialListType>
    {
        public SpecialistTypeController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<SpecialistTypes, SpecialistTypeModel, SearchSpecialListType>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
        }

        /// <summary>
        /// Download file import chuyên khoa khám
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-template-import")]
        public override async Task<ActionResult> DownloadTemplateImport()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, TEMPLATE_FOLDER_NAME, SPECIAL_LIST_TEMPLATE_NAME);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            var file = await System.IO.File.ReadAllBytesAsync(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateImport.xlsx");
        }



        #region Contants

        public const string SPECIAL_LIST_TEMPLATE_NAME = "SpecialListTypeTemplate.xlsx";

        #endregion

    }
}
