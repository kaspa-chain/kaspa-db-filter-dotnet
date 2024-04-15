using Its.Kaspa.Filter.Context;
using Its.Kaspa.Filter.Kaspa;

public class Program
{
    public static void Main(String[] args)
    {
        //using var db = new KaspaContext();

        var client = new KaspadClient();
        var blockHash = "9a51928b6741ae50dc15c6a8b3b681eec348d826fd71b392711b105e26ca4fde";

        bool isDone = false;
        int cnt = 0;

        while (!isDone)
        {
            cnt++;

            Console.WriteLine($"#### [{cnt}] Getting block [{blockHash}]");

            var getBlckTask = KaspaGrpc.GetBlocks(client, blockHash);
            var respMsg = getBlckTask.Result;

            if (respMsg.Error != null)
            {
                //Found error returned
                isDone = true;
                Console.WriteLine($"[{cnt}] Exit with error [{respMsg.Error.Message}]");
            }
            else
            {
                var blocks = respMsg.Blocks;
                var blockHashes = respMsg.BlockHashes;

                Console.WriteLine($"==== [{cnt}] Blocks size = [{blocks.Count}], BlockHashes size = [{blockHashes.Count}]");
                foreach (var block in blocks)
                {
                    Console.WriteLine($"==== [{cnt}] Block [{block.VerboseData.Hash}]");
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
