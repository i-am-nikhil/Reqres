using Microsoft.Extensions.Logging;
using Reqres.Infrastructure.Interfaces;
using Reqres.Infrastructure.Http.Models;

namespace Reqres.Application.Services
{
    /// <summary>
    /// The core use case implementation. It fetches users by delegating to the IReqresApiClient.
    /// Notice this class has no dependency on HttpClient or any caching mechanism. It only knows about the interface.
    /// </summary>
    public class ExternalUserService(IReqresApiClient apiClient, ILogger<ExternalUserService> logger) : IExternalUserService
    {
        private readonly IReqresApiClient _apiClient = apiClient;
        private readonly ILogger<ExternalUserService> _logger = logger;

        public async Task<User> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("Application Service: Getting user {UserId}", userId);
            return await _apiClient.GetUserByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Application Service: Getting all users");
            return await _apiClient.GetAllUsersAsync();
        }
    }
}