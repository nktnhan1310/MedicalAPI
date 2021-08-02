using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class MomoConfigurationService : CoreHospitalService<MomoConfigurations, BaseHospitalSearch>, IMomoConfigurationService
    {
        public MomoConfigurationService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<string> GetExistItemMessage(MomoConfigurations item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistConfiguration = await this.unitOfWork.Repository<MomoConfigurations>().GetQueryable()
                .AnyAsync(e => !e.Deleted && e.Active && e.Id != item.Id
                );
            if (isExistConfiguration)
                messages.Add("Có cấu hình đã tồn tại");
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }
    }
}
