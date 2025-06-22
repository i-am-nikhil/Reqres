using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reqres.Infrastructure.Interfaces;
using Reqres.Infrastructure.Configuration;
using Reqres.Infrastructure.Exceptions;
using Reqres.Infrastructure.Http.Models;
using System.Net;
using System.Text.Json;

namespace Reqres.Infrastructure.Http
{
    /// <summary>
    /// Implements the IReqresApiClient interface. This is where all the HttpClient logic,
    /// error handling, and data mapping happens. This is the "Adapter" in Clean Architecture.
    /// </summary>
    public class ReqresApiClient : IReqresApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReqresApiClient> _logger;

        public ReqresApiClient(HttpClient httpClient, IOptions<ReqresApiOptions> options, ILogger<ReqresApiClient> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("[API Client] User {UserId} not found (404).", userId);
                    return null;
                }

                response.EnsureSuccessStatusCode(); // Throws for non-2xx status codes

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<SingleUserApiResponse>(jsonResponse);

                // Map from infrastructure model (ApiUser) to application model (User)
                return MapToUser(apiResponse?.Data);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "[API Client] Network error for user {UserId}.", userId);
                throw new ApiClientException("A network error occurred.", ex);
            }
            // Other exceptions (like JsonException) could be caught here as well.
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var allUsers = new List<User>();
            var currentPage = 1;
            var totalPages = 1;

            while (currentPage <= totalPages)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"users?page={currentPage}");
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paginatedResponse = JsonSerializer.Deserialize<PaginatedUsersApiResponse>(jsonResponse);

                    if (paginatedResponse?.Data != null)
                    {
                        // Map from IEnumerable<ApiUser> to IEnumerable<User>
                        allUsers.AddRange(paginatedResponse.Data.Select(MapToUser));
                        totalPages = paginatedResponse.TotalPages;
                    }
                    else
                    {
                        break;
                    }
                    currentPage++;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "[API Client] Network error on page {Page}.", currentPage);
                    throw new ApiClientException("A network error occurred while fetching all users.", ex);
                }
            }
            return allUsers;
        }

        // Simple mapper method to convert from the Infrastructure DTO to the Application Model.
        private static User MapToUser(ApiUser apiUser)
        {
            if (apiUser == null) return null;
            return new User
            {
                Id = apiUser.Id,
                Email = apiUser.Email,
                FirstName = apiUser.FirstName,
                LastName = apiUser.LastName
            };
        }
    }
}