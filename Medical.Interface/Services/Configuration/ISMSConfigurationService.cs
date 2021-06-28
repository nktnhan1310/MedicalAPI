using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface
{
    public interface ISMSConfigurationService : IDomainService<SMSConfiguration, BaseSearch>
    {
        /// <summary>
        /// Gửi SMS qua SDT
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        Task<bool> SendSMS(string Phone, string Content);
    }
}
