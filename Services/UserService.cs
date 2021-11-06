using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public class UserService : IUserService
    {
        private static List<string> _users = new List<string>()
        {
            "abc",
            "cba",
            "aaa"
        };

        private const string CacheKey = "GetUsers";

        private readonly IDistributedCache _distributedCache;

        public UserService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<List<string>> GetUsers()
        {
            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            cts.Token.ThrowIfCancellationRequested();
            var users = await _distributedCache.GetAsync(CacheKey, cts.Token);

            if (users == null)
            {
                var serializedUsers = JsonConvert.SerializeObject(_users);
                var userBytes = Encoding.UTF8.GetBytes(serializedUsers);

                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                await _distributedCache.SetAsync(CacheKey, userBytes, options, cts.Token);
                return _users;
            }

            var userList = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(users));
            return userList;
        }
    }

    public interface IUserService
    {
        Task<List<string>> GetUsers();
    }
}