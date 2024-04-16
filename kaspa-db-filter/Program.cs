using Serilog;
using Its.Kaspa.Filter.Context;
using Its.Kaspa.Filter.Kaspa;
using Its.Kaspa.Filter.Config;

public class Program
{
    private static void InitLogger()
    {
        var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;
    }

    public static void Main(String[] args)
    {
        IConfig cfg = new Config("appsettings.json");
        InitLogger();

        using var db = new KaspaContext(cfg);
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
                    Log.Information($"==== [{cnt}] Block [{block.VerboseData.Hash}]");
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
