namespace Its.Kaspa.Filter.Config;

public interface IConfig
{
    public string PgHost { get; }
    public string PgPort { get; }
    public string PgDatabase { get; }
    public string PgUser { get; }
    public string PgPassword { get; }
    public string KaspaNodeUrl { get; }
    public string KaspaStartBlockHash { get; }
}
