using System.Text.Json.Serialization;

namespace Reqres.Infrastructure.Http.Models
{
    // These models are specific to the structure of the external API's JSON response.
    // They belong in the Infrastructure layer because they are an implementation detail of how we get the data.

    public class ApiUser
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }
        [JsonPropertyName("first_name")] public string FirstName { get; set; }
        [JsonPropertyName("last_name")] public string LastName { get; set; }
    }

    public class SingleUserApiResponse
    {
        [JsonPropertyName("data")] public ApiUser Data { get; set; }
    }

    public class PaginatedUsersApiResponse
    {
        [JsonPropertyName("page")] public int Page { get; set; }
        [JsonPropertyName("per_page")] public int PerPage { get; set; }
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("total_pages")] public int TotalPages { get; set; }
        [JsonPropertyName("data")] public IEnumerable<ApiUser> Data { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}