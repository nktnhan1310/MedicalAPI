using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class SystemConfigFeeService : DomainService<SystemConfigFee, BaseSearch>, ISystemConfigFeeService
    {
        public SystemConfigFeeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<string> GetExistItemMessage(SystemConfigFee item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistItem = await this.unitOfWork.Repository<SystemConfigFee>().GetQueryable()
                .AnyAsync(e => !e.Deleted && e.Active && e.Id != item.Id);
            if (isExistItem)
                messages.Add("Cấu hình đã tồn tại");
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

    }
}
