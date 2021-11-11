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
    public class HospitalHolidayConfigService : DomainService<HospitalHolidayConfigs, BaseHospitalSearch>, IHospitalHolidayConfigService
    {
        public HospitalHolidayConfigService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Kiểm tra cấu hình ngày nghỉ đã tồn tại chưa?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(HospitalHolidayConfigs item)
        {
            // Kiểm tra khoảng thời gian nghỉ đã tồn tại chưa?
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistHolidayConfig = await this.Queryable.AnyAsync(e => !e.Deleted && e.Active
            && e.Id != item.Id
            && e.FromDate <= item.FromDate && e.ToDate >= item.ToDate
            && e.FromDate <= item.ToDate && e.ToDate >= item.ToDate
            );
            if (isExistHolidayConfig)
                result = string.Format("Cấu hình từ ngày {0} đến ngày {1}"
                    , item.FromDate.Value.ToString("dd/MM/yyyy")
                    , item.ToDate.Value.ToString("dd/MM/yyyy"));
            return result;
        }
    }
}
