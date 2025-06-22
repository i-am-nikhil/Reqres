// =================================================================
// FILE: Reqres.Infrastructure/Configuration/ReqresApiOptions.cs
// =================================================================
namespace Reqres.Infrastructure.Configuration
{
    /// <summary>
    /// BONUS: Implements the Options Pattern for strong-typed configuration.
    /// This object will be populated from the "ReqresApi" section of appsettings.json.
    /// </summary>
    public class ReqresApiOptions
    {
        public const string ConfigurationSectionName = "ReqresApi";
        public string BaseUrl { get; set; }
        public int CacheDurationSeconds { get; set; }
    }
}

