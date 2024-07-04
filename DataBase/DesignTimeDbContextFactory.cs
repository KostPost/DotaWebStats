using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DotaWebStatsContext>
{
    public DotaWebStatsContext CreateDbContext(string[] args)
    {
        const string connectionString = "Host=localhost;Database=DotaWebStats;Username=postgres;Password=2025";
        return new DotaWebStatsContext(connectionString);
    }
}