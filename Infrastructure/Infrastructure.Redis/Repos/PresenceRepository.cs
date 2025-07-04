using Microsoft.Extensions.Logging;
using Nonuso.Domain.IRepos;
using StackExchange.Redis;

namespace Nonuso.Infrastructure.Redis.Repos
{
    internal class PresenceRepository(IConnectionMultiplexer redis, ILogger<PresenceRepository> logger) : IPresenceRepository
    {
        private readonly IDatabase _dbRedis = redis.GetDatabase();
        private readonly ILogger<PresenceRepository> _logger = logger; 

        public async Task<(bool isOnline, string currentPage)?> GetUserPresenceAsync(Guid userId)
        {
            var userPresence = await _dbRedis.HashGetAllAsync(key: userId.ToString());

                return userPresence?.Length > 0 ? (true, userPresence[0].ToString())
                    : null;         
        }

        public async Task SetUserOfflineAsync(Guid userId)
        {
            await _dbRedis.KeyDeleteAsync($"{userId}");
        }

        public async Task SetUserOnlineAsync(Guid userId, string page)
        {
            var key = $"{userId}";
            await _dbRedis.HashSetAsync(key,
                [
                    new HashEntry("page", page)
                ]);

            await _dbRedis.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
        }

        public async Task UpdatePageAsync(Guid userId, string page)
        {
            var key = $"{userId}";
            if (await _dbRedis.KeyExistsAsync(key))
            {
                await _dbRedis.HashSetAsync(key,
                [
                    new HashEntry("page", page)
                ]);

                await _dbRedis.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
            }
            else
            {
                await SetUserOnlineAsync(userId, page);
            }
        }
    }
}
