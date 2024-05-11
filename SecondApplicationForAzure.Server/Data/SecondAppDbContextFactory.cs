using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using SecondApplicationForAzure.Model;

namespace SecondApplicationForAzure.Server.Data;

public class SecondAppDbContextFactory : IDesignTimeDbContextFactory<SecondAppDbContext>
{
    public SecondAppDbContext CreateDbContext(string[]? args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<SecondAppDbContext>();

        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(SecondAppDbContextFactory).Assembly.FullName));

        return new SecondAppDbContext(optionsBuilder.Options);
    }
}