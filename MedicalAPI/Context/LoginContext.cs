using MedicalAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MedicalAPI.Model;

namespace MedicalAPI.Context
{
    public sealed class LoginContext
    {
        private static LoginContext instance = null;

        private LoginContext()
        {
        }

        public static LoginContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginContext();
                }
                return instance;
            }
        }

        public UserLoginModel CurrentUser
        {
            get
            {
                var user = (UserLoginModel)Utils.HttpContext.Current.Items["User"];
                if (user != null)
                {
                    return user;
                    //var claim = Utils.HttpContext.Current.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.UserData);
                    //if (claim != null)
                    //{
                    //    return JsonConvert.DeserializeObject<UserLoginModel>(claim.Value);
                    //}
                }
                return null;
            }
        }

        public void Clear()
        {
            instance = null;
        }

        public UserLoginModel GetCurrentUser(IHttpContextAccessor httpContext)
        {
            if (httpContext != null && httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var claim = httpContext.HttpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.UserData);
                if (claim != null)
                {
                    return JsonConvert.DeserializeObject<UserLoginModel>(claim.Value);
                }
            }
            return null;
        }
    }
}
