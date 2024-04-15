namespace Its.Kaspa.Filter.Kaspa;

public interface IKaspaGrpc
{
    public Task<GetBlocksResponseMessage> GetBlocks(IClient client, string blockHash);
}
