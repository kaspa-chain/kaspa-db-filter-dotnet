using Microsoft.EntityFrameworkCore;
using Its.Kaspa.Filter.Models;
using Its.Kaspa.Filter.Config;

namespace Its.Kaspa.Filter.Context;

public class KaspaContext : DbContext
{
    public DbSet<Block> blocks { get; set; }
    private string pgHost;
    private string pgPort;
    private string pgUser;
    private string pgPassword;
    private string pgDatabase;

    public KaspaContext(IConfig cfg)
    {
        pgHost = cfg.PgHost;
        pgPort = cfg.PgPort;
        pgUser = cfg.PgUser;
        pgPassword = cfg.PgPassword;
        pgDatabase = cfg.PgDatabase;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"host={pgHost};port={pgPort};database={pgDatabase};user id={pgUser};password={pgPassword};");
}
