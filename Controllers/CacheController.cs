using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace caching.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;

        public CacheController(IMemoryCache cache, IUserService userService)
        {
            _cache = cache;
            _userService = userService;
        }

        [HttpGet("distributedCache")]
        public IActionResult GetUsers()
        {
            return new JsonResult(new {users = _userService.GetUsers()}) {StatusCode = StatusCodes.Status200OK};
        }

        [HttpGet]
        public IActionResult GetUser(string user)
        {
            var found = _cache.TryGetValue(user, out int count);
            if (!found)
            {
                count = 0;
            }
            else
            {
                count++;
            }

            _cache.Set(user, count, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddHours(1),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });


            return new JsonResult(new {user, count});
        }
    }
}