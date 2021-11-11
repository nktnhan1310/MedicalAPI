using Medical.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class TokenManagerService : ITokenManagerService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService userService;
        //private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenManagerService(IDistributedCache cache,
                IHttpContextAccessor httpContextAccessor
                //IServiceProvider serviceProvider
            )
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            //userService = serviceProvider.GetRequiredService<IUserService>();
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        //public async Task<bool> IsActiveAsync(string token)
        //    => await _cache.GetStringAsync(GetKey(token)) == null;

        public async Task<bool> IsActiveAsync(string token)
        {
            bool result = await _cache.GetStringAsync(GetKey(token)) == null;
            return result;
        }

        public async Task DeactivateAsync(string token)
            => await _cache.SetStringAsync(GetKey(token),
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromHours(24)
                });

        private string GetCurrentAsync()
        {
            StringValues result = string.Empty;
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];
            result = authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
            return result;
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
    }
}
