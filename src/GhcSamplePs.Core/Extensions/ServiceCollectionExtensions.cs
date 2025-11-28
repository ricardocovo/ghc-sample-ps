using GhcSamplePs.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GhcSamplePs.Core.Extensions;

/// <summary>
/// Extension methods for configuring Entity Framework Core services in the application's dependency injection container.
/// </summary>
/// <remarks>
/// This class provides a convenient way to register the <see cref="ApplicationDbContext"/> with proper
/// configuration including retry policies for transient failures.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ApplicationDbContext to the service collection with SQL Server configuration,
    /// connection retry policies for transient failures, and development-specific options.
    /// </summary>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <param name="enableSensitiveDataLogging">
    /// Whether to enable sensitive data logging. Should only be true in development environments.
    /// Defaults to false.
    /// </param>
    /// <param name="enableDetailedErrors">
    /// Whether to enable detailed error messages. Should only be true in development environments.
    /// Defaults to false.
    /// </param>
    /// <param name="maxRetryCount">
    /// The maximum number of retry attempts for transient failures. Defaults to 5.
    /// </param>
    /// <param name="maxRetryDelaySeconds">
    /// The maximum delay between retry attempts in seconds. Defaults to 30.
    /// </param>
    /// <param name="commandTimeoutSeconds">
    /// The command timeout in seconds. Defaults to 30.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="connectionString"/> is null or whitespace.
    /// </exception>
    /// <example>
    /// <code>
    /// // In Program.cs for development
    /// builder.Services.AddApplicationDbContext(
    ///     connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
    ///     enableSensitiveDataLogging: builder.Environment.IsDevelopment(),
    ///     enableDetailedErrors: builder.Environment.IsDevelopment());
    /// 
    /// // In Program.cs for production with custom retry settings
    /// builder.Services.AddApplicationDbContext(
    ///     connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
    ///     enableSensitiveDataLogging: false,
    ///     enableDetailedErrors: false,
    ///     maxRetryCount: 10,
    ///     maxRetryDelaySeconds: 60);
    /// </code>
    /// </example>
    public static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        string connectionString,
        bool enableSensitiveDataLogging = false,
        bool enableDetailedErrors = false,
        int maxRetryCount = 5,
        int maxRetryDelaySeconds = 30,
        int commandTimeoutSeconds = 30)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                // Configure connection retry policies for transient failures
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: maxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(maxRetryDelaySeconds),
                    errorNumbersToAdd: null);

                // Set command timeout
                sqlServerOptions.CommandTimeout(commandTimeoutSeconds);

                // Configure query splitting for related data (improves performance for complex queries)
                sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            if (enableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }

            if (enableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}
