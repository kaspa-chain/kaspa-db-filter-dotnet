using Serilog;
using Its.Kaspa.Filter.Context;
using Its.Kaspa.Filter.Kaspa;
using Its.Kaspa.Filter.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static void InitLogger()
    {
        var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;
    }

    private static IHostBuilder CreateDefaultBuilder(Config cfg)
    {
        var connStr = $"host={cfg.PgHost};port={cfg.PgPort};database={cfg.PgDatabase};user id={cfg.PgUser};password={cfg.PgPassword};";

        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<IConfig, Config>(x => cfg);
                services.AddDbContext<KaspaContext>(options => options.UseNpgsql(connStr));
            });
    }

    public static void Main(String[] args)
    {
        InitLogger();
        var cfg = new Config("appsettings.json");

        var host = CreateDefaultBuilder(cfg).Build();
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<KaspaContext>();
            db.Database.Migrate();
        }

        var client = new KaspadClient(cfg);
        var kaspaGrpc = new KaspaGrpc();

        var blockHash = cfg.KaspaStartBlockHash;

        bool isDone = false;
        int cnt = 0;

        while (!isDone)
        {
            cnt++;

            Log.Information($"#### [{cnt}] Getting block [{blockHash}]");

            var getBlckTask = kaspaGrpc.GetBlocks(client, blockHash);
            var respMsg = getBlckTask.Result;

            if (respMsg.Error != null)
            {
                //Found error returned
                isDone = true;

                Log.Information($"[{cnt}] Exit with error [{respMsg.Error.Message}]");
            }
            else
            {
                var blocks = respMsg.Blocks;
                var blockHashes = respMsg.BlockHashes;

                Log.Information($"==== [{cnt}] Blocks size = [{blocks.Count}], BlockHashes size = [{blockHashes.Count}]");
                foreach (var block in blocks)
                {
                    var transactions = block.Transactions;
                    Log.Information($"==== [{cnt}] Block [{block.VerboseData.Hash}], Tx Count = [{transactions.Count}]");
                }

                if (blockHashes.Count > 0)
                {
                    blockHash = blockHashes[blockHashes.Count-1];
                }
            }

            Thread.Sleep(2000);
        }
    }
}
