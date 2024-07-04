using Microsoft.EntityFrameworkCore;
using ModelClasses;

namespace DataBase;

public class DotaWebStatsContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }

    private readonly string _connectionString = "Host=localhost;Database=DotaWebStats;Username=postgres;Password=2025";

    public DotaWebStatsContext(DbContextOptions<DotaWebStatsContext> options) : base(options) { }

    public DotaWebStatsContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DotaWebStatsContext()
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("user_account");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserName).HasColumnName("user_name");
            entity.Property(e => e.SteamId).HasColumnName("steam_id");
            entity.Property(e => e.CurrentMMR).HasColumnName("current_mmr");
            entity.Property(e => e.GoalMMR).HasColumnName("goal_mmr");
        });
    }

}