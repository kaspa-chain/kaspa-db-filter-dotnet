using Grpc.Core;

namespace Its.Kaspa.Filter.Kaspa;

public static class KaspaGrpc
{
    public static async Task<GetBlocksResponseMessage> GetBlocks(IClient client, string blockHash)
    {
        using var messageStream = client.GetRpcClient().MessageStream();
        var request = new KaspadRequest
        {
            GetBlocksRequest = new GetBlocksRequestMessage()
            {
                LowHash = blockHash,
                IncludeTransactions = true,
                IncludeBlocks = true,
            }
        };

        await messageStream.RequestStream.WriteAsync(request);
        await messageStream.ResponseStream.MoveNext();
        var response = messageStream.ResponseStream.Current;
        var result = response.GetBlocksResponse;

        return result;
    }
}
