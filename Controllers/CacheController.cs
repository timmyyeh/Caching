using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace caching.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public CacheController(IMemoryCache cache)
        {
            _cache = cache;
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