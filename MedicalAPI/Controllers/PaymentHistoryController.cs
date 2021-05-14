using Medical.Entities;
using Medical.Interface.Services;
using MedicalAPI.Model;
using Microsoft.AspNetCore.Hosting;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý lịch sử thanh toán")]
    public class PaymentHistoryController : BaseController<PaymentHistories, PaymentHistoryModel, BaseSearch>
    {
        public PaymentHistoryController(IServiceProvider serviceProvider, ILogger<BaseController<PaymentHistories, PaymentHistoryModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
        }
    }
}
