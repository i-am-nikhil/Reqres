// Reqres.Client.Demo/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using Reqres.Application.Services;
using Reqres.Infrastructure.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Reqres.Infrastructure.Configuration;
using Reqres.Infrastructure.Caching;
using Reqres.Infrastructure.Interfaces;
using Microsoft.Extensions.Http; // For AddHttpClient
using Microsoft.Extensions.Caching.Memory; // For AddMemoryCache
using Scrutor;



namespace Reqres.Client.Demo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // BONUS: Advanced Configuration - Bind appsettings to a strong-typed options class.
                    services.Configure<ReqresApiOptions>(context.Configuration.GetSection(ReqresApiOptions.ConfigurationSectionName));

                    // 1. Register the underlying API client implementation with Polly for resilience.
                    // BONUS: Implement actual retry logic using Polly.
                    services.AddHttpClient<IReqresApiClient, ReqresApiClient>().AddPolicyHandler(GetRetryPolicy());


                    // 2. Register the core application service.
                    services.AddScoped<IExternalUserService, ExternalUserService>();

                    // 3. Register the caching decorator.
                    // This uses Scrutor's decoration capabilities to wrap the IExternalUserService.
                    // When IExternalUserService is requested, DI will provide CachingExternalUserServiceDecorator,
                    // which itself receives the original ExternalUserService.
                    services.Decorate<IExternalUserService, CachingExternalUserServiceDecorator>();

                    // Add memory caching services
                    services.AddMemoryCache();

                    // Add the demo runner
                    services.AddTransient<AppRunner>();
                })
                .Build();

            var appRunner = host.Services.GetRequiredService<AppRunner>();
            await appRunner.RunAsync();
        }

        /// <summary>
        /// Defines the Polly retry policy.
        /// </summary>
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Retry 3 times with an exponential backoff (1, 2, 4 seconds)
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles HttpRequestException, 5xx, 408
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Using a logger is better, but Console works for this demo.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[Polly] Retrying request... Attempt: {retryAttempt}. Delay: {timespan.TotalSeconds}s. Reason: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                        Console.ResetColor();
                    });
        }
    }

    /// <summary>
    /// The application runner remains largely the same, but now it requests the top-level
    /// IExternalUserService and gets the decorated, cached, and resilient version.
    /// </summary>
    public class AppRunner
    {
        private readonly IExternalUserService _userService;
        private readonly ILogger<AppRunner> _logger;

        public AppRunner(IExternalUserService userService, ILogger<AppRunner> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("\n--- Clean Architecture API Client Demo ---");

            // First run - should be a cache miss and hit the API
            _logger.LogInformation("\n[RUN 1] Fetching all users (expect cache MISS)...");
            await _userService.GetAllUsersAsync();

            // Second run - should be a cache hit
            _logger.LogInformation("\n[RUN 2] Fetching all users again (expect cache HIT)...");
            var allUsers = await _userService.GetAllUsersAsync();
            _logger.LogInformation("Got {UserCount} users from service.", allUsers.Count());

            // First run - user 2
            _logger.LogInformation("\n[RUN 1] Fetching user 2 (expect cache MISS)...");
            await _userService.GetUserByIdAsync(2);

            // Second run - user 2
            _logger.LogInformation("\n[RUN 2] Fetching user 2 again (expect cache HIT)...");
            await _userService.GetUserByIdAsync(2);

            _logger.LogInformation("\n--- Demo Finished ---");
        }
    }
}
