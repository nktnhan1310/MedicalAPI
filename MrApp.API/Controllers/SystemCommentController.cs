using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemCommentController : BaseController
    {
        private ISystemCommentService systemCommentService;
        public SystemCommentController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            systemCommentService = serviceProvider.GetRequiredService<ISystemCommentService>();
        }

        /// <summary>
        /// Gửi liên hệ cho hệ thống
        /// </summary>
        /// <param name="systemCommentModel"></param>
        /// <returns></returns>
        [HttpPost("create-comment-system")]
        public async Task<AppDomainResult> CreateComment([FromBody] SystemCommentModel systemCommentModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                systemCommentModel.Created = DateTime.Now;
                systemCommentModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                systemCommentModel.Active = true;
                systemCommentModel.UserId = LoginContext.Instance.CurrentUser.UserId;
                var item = mapper.Map<SystemComments>(systemCommentModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.systemCommentService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.systemCommentService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

    }
}
