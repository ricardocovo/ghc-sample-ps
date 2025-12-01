using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GhcSamplePs.Core.Data;

/// <summary>
/// Design-time factory for creating instances of <see cref="ApplicationDbContext"/>.
/// This is used by EF Core tools (e.g., migrations) when no runtime DbContext is available.
/// </summary>
/// <remarks>
/// <para>
/// This factory provides a way to create DbContext instances at design time for:
/// - Creating migrations with dotnet ef migrations add
/// - Generating database scripts with dotnet ef migrations script
/// - Other EF Core tooling commands
/// </para>
/// <example>
/// <code>
/// // This factory is automatically discovered by EF Core tools
/// // Run migrations from the solution root:
/// dotnet ef migrations add MigrationName --project src/GhcSamplePs.Core --startup-project src/GhcSamplePs.Web
/// </code>
/// </example>
/// </remarks>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a new instance of <see cref="ApplicationDbContext"/> for design-time operations.
    /// </summary>
    /// <param name="args">Arguments passed from the command line.</param>
    /// <returns>A new instance of <see cref="ApplicationDbContext"/>.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use a placeholder connection string for design-time operations
        // The actual connection string will be provided at runtime
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GhcSamplePs_DesignTime;Trusted_Connection=True;");

        return new ApplicationDbContext(optionsBuilder.Options, "design-time-user");
    }
}
