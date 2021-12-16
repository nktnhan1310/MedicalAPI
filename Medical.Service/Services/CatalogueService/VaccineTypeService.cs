using AutoMapper;
using Medical.Entities;
<<<<<<< HEAD
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Text;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

namespace Medical.Service
{
    public class VaccineTypeService : CatalogueHospitalService<VaccineTypes, BaseHospitalSearch>, IVaccineTypeService
    {
<<<<<<< HEAD
        public VaccineTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }

        /// <summary>
        /// Thêm mới loại vaccine
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(VaccineTypes item)
        {

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<VaccineTypes>().Create(item);
                    await this.unitOfWork.SaveAsync();
                    // THÊM MỚI THÔNG TIN CHI TIẾT CỦA LOẠI VACCINE
                    if (item.VaccineTypeDetails != null && item.VaccineTypeDetails.Any())
                    {
                        foreach (var vaccineTypeDetail in item.VaccineTypeDetails)
                        {
                            vaccineTypeDetail.VaccineTypeId = item.Id;
                            this.unitOfWork.Repository<VaccineTypeDetails>().Create(vaccineTypeDetail);
                        }
                    }
                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin loại vaccine
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(VaccineTypes item)
        {

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    this.unitOfWork.Repository<VaccineTypes>().Update(item);

                    // CẬP NHẬT THÔNG TIN CHI TIẾT LOẠI VACCINE
                    if (item.VaccineTypeDetails != null && item.VaccineTypeDetails.Any())
                    {
                        foreach (var vaccineTypeDetail in item.VaccineTypeDetails)
                        {
                            var existVaccineTypeDetail = await this.unitOfWork.Repository<VaccineTypeDetails>().GetQueryable()
                                .Where(e => e.Id == vaccineTypeDetail.Id).FirstOrDefaultAsync();
                            if(existVaccineTypeDetail != null)
                            {
                                vaccineTypeDetail.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                                vaccineTypeDetail.VaccineTypeId = item.Id;
                                this.unitOfWork.Repository<VaccineTypeDetails>().Update(vaccineTypeDetail);
                            }
                            else
                            {
                                vaccineTypeDetail.VaccineTypeId = item.Id;
                                this.unitOfWork.Repository<VaccineTypeDetails>().Create(vaccineTypeDetail);
                            }
                        }
                    }
                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }
=======
        public VaccineTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
    }
}
