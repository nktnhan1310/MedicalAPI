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
    public class DoctorController : CoreHospitalController<Doctors, DoctorModel, SearchDoctor>
    {
        private readonly IDoctorDetailService doctorDetailService;
        public DoctorController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<Doctors, DoctorModel, SearchDoctor>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDoctorService>();
            this.doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
        }

        /// <summary>
        /// Lấy thông tin theo bác sĩ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {

            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);

            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<DoctorModel>(item);
                    var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.DoctorId == id);
                    var doctorDetailModels = mapper.Map<IList<DoctorDetailModel>>(doctorDetails);
                    itemModel.DoctorDetails = doctorDetailModels;
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }
    }
}
