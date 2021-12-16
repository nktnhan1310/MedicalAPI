using AutoMapper;
using Medical.Entities;
<<<<<<< HEAD
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.DbContext;
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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

namespace Medical.Service
{
    public class AllergyTypeService : CatalogueService<AllergyTypes, BaseSearch>, IAllergyTypeService
    {
<<<<<<< HEAD
        public AllergyTypeService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

        /// <summary>
        /// Thêm mới loại dị ứng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(AllergyTypes item)
        {
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<AllergyTypes>().Create(item);
                    await this.unitOfWork.SaveAsync();
                    if (item.AllergyDescriptionTypes != null && item.AllergyDescriptionTypes.Any())
                    {
                        foreach (var allergyDescriptionType in item.AllergyDescriptionTypes)
                        {
                            allergyDescriptionType.Active = true;
                            allergyDescriptionType.Deleted = false;
                            allergyDescriptionType.AllergyTypeId = item.Id;
                            this.unitOfWork.Repository<AllergyDescriptionTypes>().Create(allergyDescriptionType);
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
        /// Cập nhật thông tin loại dị ứng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(AllergyTypes item)
        {
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existItem = await this.unitOfWork.Repository<AllergyTypes>().GetQueryable()
                        .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
                    if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
                    existItem = mapper.Map<AllergyTypes>(item);
                    existItem.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    this.unitOfWork.Repository<AllergyTypes>().Update(existItem);

                    // Cập nhật thông tin mô tả của dị ứng
                    if (item.AllergyDescriptionTypes != null && item.AllergyDescriptionTypes.Any())
                    {
                        foreach (var allergyDescriptionType in item.AllergyDescriptionTypes)
                        {
                            var existAllergyDescriptionType = await this.unitOfWork.Repository<AllergyDescriptionTypes>().GetQueryable()
                                .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
                            if (existAllergyDescriptionType != null)
                            {
                                existAllergyDescriptionType = mapper.Map<AllergyDescriptionTypes>(allergyDescriptionType);
                                existAllergyDescriptionType.AllergyTypeId = item.Id;
                                existAllergyDescriptionType.Updated = DateTime.Now;
                                this.unitOfWork.Repository<AllergyDescriptionTypes>().Update(existAllergyDescriptionType);
                            }
                            else
                            {
                                allergyDescriptionType.Active = true;
                                allergyDescriptionType.Deleted = false;
                                allergyDescriptionType.AllergyTypeId = item.Id;
                                this.unitOfWork.Repository<AllergyDescriptionTypes>().Create(allergyDescriptionType);
                            }
                        }
                    }

                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(AllergyTypes item)
        {
            string result = string.Empty;
            bool isExistCode = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Code == item.Code);

            if (isExistCode)
                return "Mã đã tồn tại!";
            return result;
        }
=======
        public AllergyTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
    }
}
