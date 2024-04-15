using Microsoft.EntityFrameworkCore;
using Its.Kaspa.Filter.Models;

namespace Its.Kaspa.Filter.Context;

public class KaspaContext : DbContext
{
    public DbSet<Block> blocks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("host=localhost;database=test_db;user id=postgres;password=Dotnet@70;");
}
