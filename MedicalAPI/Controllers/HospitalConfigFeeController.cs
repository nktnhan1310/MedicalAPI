using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/hospital-config-fee")]
    [ApiController]
    [Description("Quản lý danh mục cấu hình phí thanh toán theo từng bệnh viện")]
    public class HospitalConfigFeeController : BaseController<HospitalConfigFees, HospitalConfigFeeModel, SearchHospitalConfigFee>
    {
        public HospitalConfigFeeController(IServiceProvider serviceProvider, ILogger<BaseController<HospitalConfigFees, HospitalConfigFeeModel, SearchHospitalConfigFee>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IHospitalConfigFeeService>();
        }
    }
}
