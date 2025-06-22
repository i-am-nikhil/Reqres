
using Reqres.Infrastructure.Http.Models;

namespace Reqres.Infrastructure.Interfaces
{
    /// <summary>
    /// Defines the contract for a client that communicates directly with the Reqres API.
    /// This is an "outbound port" in Clean Architecture. The implementation will be in the Infrastructure layer.
    /// </summary>
    public interface IReqresApiClient
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}

namespace Reqres.Application.Services
{
    /// <summary>
    /// Defines the main application service contract.
    /// This is the primary interface the presentation layer will interact with.
    /// It orchestrates the logic using the API client.
    /// </summary>
    public interface IExternalUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}