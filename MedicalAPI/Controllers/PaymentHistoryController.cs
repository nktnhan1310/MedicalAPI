using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAPI.Controllers
{
    [Route("api/payment-history")]
    [ApiController]
    [Description("Quản lý lịch sử thanh toán")]
    [Authorize]
    public class PaymentHistoryController : BaseController<PaymentHistories, PaymentHistoryModel, BaseSearch>
    {
        public PaymentHistoryController(IServiceProvider serviceProvider, ILogger<BaseController<PaymentHistories, PaymentHistoryModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
        }
    }
}
