using Microsoft.EntityFrameworkCore;
using Its.Kaspa.Filter.Models;
using Its.Kaspa.Filter.Config;

namespace Its.Kaspa.Filter.Context;

public class KaspaContext : DbContext
{
    public DbSet<Block> Blocks { get; set; }

    private string pgHost = "localhost";
    private string pgPort = "5432";
    private string pgUser = "postgres";
    private string pgPassword = "postgresPassword";
    private string pgDatabase = "kaspa-fliter";

    public KaspaContext(DbContextOptions<KaspaContext> options, IConfig cfg) : base(options) 
    {
        pgHost = cfg.PgHost;
        pgPort = cfg.PgPort;
        pgUser = cfg.PgUser;
        pgPassword = cfg.PgPassword;
        pgDatabase = cfg.PgDatabase;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"host={pgHost};port={pgPort};database={pgDatabase};user id={pgUser};password={pgPassword};");
}
