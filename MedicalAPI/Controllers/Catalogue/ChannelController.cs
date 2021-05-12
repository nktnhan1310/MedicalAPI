using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface.Services;
using MedicalAPI.Model;
using MedicalAPI.Model.DomainModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Kênh đăng ký khám bệnh")]
    public class ChannelController : CatalogueController<Channels, ChannelModel, BaseSearch>
    {
        public ChannelController(IServiceProvider serviceProvider, ILogger<CatalogueController<Channels, ChannelModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IChannelService>();
        }
    }
}
