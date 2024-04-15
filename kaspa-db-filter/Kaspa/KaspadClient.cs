using Grpc.Net.Client;

namespace Its.Kaspa.Filter.Kaspa;

public class KaspadClient : RPC.RPCClient, IDisposable, IClient
{
    private readonly RPC.RPCClient client;
    
    private GrpcChannel channel;
    
    public KaspadClient()  
    {
        //var url = "https://grpc-dev.kaspa-chain.net:16110";
        var url = "https://node.kaspium.io:16110";

        channel = GrpcChannel.ForAddress(url!);
        client = new RPC.RPCClient(channel);
    }

    public RPC.RPCClient GetRpcClient()
    {
        return client;
    }

    public void Dispose()
    {
        channel.Dispose();
    }
}
