using Serilog;
using Microsoft.Extensions.Configuration;

namespace Its.Kaspa.Filter.Config;

public class Config : IConfig
{
    public Config(string appsSetting)
    {
        var builder = new ConfigurationBuilder()
                .AddJsonFile(appsSetting, true, true)
                .AddEnvironmentVariables();
    
        var config = builder.Build();

        KaspaNodeUrl = config["KaspaNodeUrl"]!;
        KaspaStartBlockHash = config["KaspaStartBlockHash"]!;

        PgHost = config["PgHost"]!;
        PgPort = config["PgPort"]!;
        PgPassword = config["PgPassword"]!;
        PgDatabase = config["PgDatabase"]!;
        PgUser = config["PgUser"]!;

        Log.Information($"PgHost=[{PgHost}]");
        Log.Information($"PgPort=[{PgPort}]");
        Log.Information($"PgDatabase=[{PgDatabase}]");
        Log.Information($"PgUser=[{PgUser}]");
        Log.Information($"KaspaNodeUrl=[{KaspaNodeUrl}]");
        Log.Information($"KaspaStartBlockHash=[{KaspaStartBlockHash}]");
    }

    public string PgHost { get; }
    public string PgPort { get; }
    public string PgDatabase { get; }
    public string PgUser { get; }
    public string PgPassword { get; }
    public string KaspaNodeUrl { get; }
    public string KaspaStartBlockHash { get; }
}