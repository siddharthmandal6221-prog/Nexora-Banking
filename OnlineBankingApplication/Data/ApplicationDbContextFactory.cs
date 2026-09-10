using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OnlineBankingApplication.Data
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: false)
                .AddJsonFile(
                    "appsettings.Development.json",
                    optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection was not found in appsettings.json.");
            }

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

            return new ApplicationDbContext(
                optionsBuilder.Options);
        }
    }
}