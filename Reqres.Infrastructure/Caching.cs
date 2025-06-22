// =================================================================
// FILE: Reqres.Infrastructure/Caching/CachingExternalUserServiceDecorator.cs
// =================================================================
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reqres.Application.Services;
using Reqres.Infrastructure.Configuration;
using Reqres.Infrastructure.Http.Models;

namespace Reqres.Infrastructure.Caching
{
    /// <summary>
    /// BONUS: Implements caching using the Decorator Pattern.
    /// This class wraps an IExternalUserService implementation and adds caching logic
    /// before delegating the call to the actual service. This keeps caching separate from the core logic.
    /// </summary>
    public class CachingExternalUserServiceDecorator : IExternalUserService
    {
        private readonly IExternalUserService _decorated;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CachingExternalUserServiceDecorator> _logger;
        private readonly ReqresApiOptions _options;
        
        private const string AllUsersCacheKey = "AllUsers";

        public CachingExternalUserServiceDecorator(IExternalUserService decorated, IMemoryCache memoryCache, IOptions<ReqresApiOptions> options, ILogger<CachingExternalUserServiceDecorator> logger)
        {
            _decorated = decorated;
            _memoryCache = memoryCache;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            string cacheKey = $"User_{userId}";
            
            // Try to get the user from the cache.
            if (_memoryCache.TryGetValue(cacheKey, out User cachedUser))
            {
                _logger.LogInformation("Cache HIT for user {UserId}", userId);
                return cachedUser;
            }

            _logger.LogInformation("Cache MISS for user {UserId}", userId);
            
            // If not in cache, call the decorated service.
            var user = await _decorated.GetUserByIdAsync(userId);

            // Add the result to the cache with a configurable expiration.
            if (user != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.CacheDurationSeconds));
                _memoryCache.Set(cacheKey, user, cacheEntryOptions);
            }
            
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            if (_memoryCache.TryGetValue(AllUsersCacheKey, out IEnumerable<User> cachedUsers))
            {
                _logger.LogInformation("Cache HIT for all users list");
                return cachedUsers;
            }

            _logger.LogInformation("Cache MISS for all users list");
            var users = await _decorated.GetAllUsersAsync();

            if (users != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.CacheDurationSeconds));
                _memoryCache.Set(AllUsersCacheKey, users, cacheEntryOptions);
            }

            return users;
        }
    }
}