using Google.Apis.Auth;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private IMedicalUnitOfWork unitOfWork;
        public GoogleAuthService(IMedicalUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// VerifyToken
        /// </summary>
        /// <param name="googleAuths"></param>
        /// <returns></returns>
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleAuths googleAuths)
        {
            try
            {
                var googleSettingInfo = await this.unitOfWork.Repository<GoogleSettings>().GetQueryable().Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();
                if(googleSettingInfo != null)
                {
                    var settings = new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string>() { googleSettingInfo.ClientId }

                    };
                    var payload = await GoogleJsonWebSignature.ValidateAsync(googleAuths.IdToken, settings);
                    return payload;
                }
                return null;
            }
            catch (Exception ex)
            {
                //log an exception
                return null;
            }
        }

    }
}
