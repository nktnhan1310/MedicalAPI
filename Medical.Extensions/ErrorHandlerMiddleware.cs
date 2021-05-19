using Medical.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Medical.Extensions
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<ControllerBase> _logger;


        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ControllerBase> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case UnauthorizedAccessException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case InvalidCastException e:
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    case EntryPointNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        {
                            var RouteData = context.Request.Path.Value.Split("/");
                            var controllerName = RouteData[2];
                            var actionName = RouteData[3];

                            //var controllerActionDescriptor = context
                            //.GetEndpoint()
                            //.Metadata
                            //.GetMetadata<ControllerActionDescriptor>();

                            //var controllerName = controllerActionDescriptor.ControllerName;
                            //var actionName = controllerActionDescriptor.ActionName;
                            // unhandled error
                            _logger.LogError(string.Format("{0} {1}: {2}", controllerName
                                , actionName, error?.Message));
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }

                        break;
                }
                var result = JsonSerializer.Serialize(new AppDomainResult()
                {
                    ResultCode = response.StatusCode,
                    ResultMessage = error?.Message,
                    Success = false
                });
                await response.WriteAsync(result);
            }
        }
    }
}
