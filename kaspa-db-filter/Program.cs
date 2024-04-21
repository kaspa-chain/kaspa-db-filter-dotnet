using Serilog;
using Its.Kaspa.Filter.Context;
using Its.Kaspa.Filter.Kaspa;
using Its.Kaspa.Filter.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//genesis - 58c2d4199e21f910d1571d114969cecef48f09f934d42ccb6a281a15868f2999

public class Program
{
    private static void InitLogger()
    {
        var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;
    }

//"https://node.kaspium.io:16110",
    public static string UnixTimeStampToDateTime(double unixTimeStamp)
    {
        TimeSpan time = TimeSpan.FromMilliseconds(unixTimeStamp);
        DateTime dateTime = new DateTime(1970, 1, 1) + time;

        return dateTime.ToString("yyyy/MM/dd hh:mm:ss tt");
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

                var blockItSelf = blocks[0];
                var selectedParentHash = blockItSelf.VerboseData.SelectedParentHash;
                
                var blueScore = blockItSelf.VerboseData.BlueScore;
                var daaScore = blockItSelf.Header.DaaScore;
                var parentCount = blockItSelf.Header.Parents[0].ParentHashes.Count;
                var blocksParentCount = blockItSelf.Header.Parents.Count;
                var blockTs = UnixTimeStampToDateTime(blockItSelf.Header.Timestamp);

                blockHash = selectedParentHash;

                Log.Information($"==== [{cnt}] [{blockTs}] Blocks size = [{blocks.Count}], Parent Count = [{parentCount}/{blocksParentCount}]");
                Log.Information($"==== [{cnt}] BlueScore = [{blueScore}], DaaScore=[{daaScore}]");
                Log.Information($"==== [{cnt}] Selected Parent Hash = [{selectedParentHash}]");

/*
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
*/
            }

            int x = 0;
            if (Int32.TryParse(cfg.DelayMs, out x))
            {
                Thread.Sleep(x);
            }
        }
    }
}
