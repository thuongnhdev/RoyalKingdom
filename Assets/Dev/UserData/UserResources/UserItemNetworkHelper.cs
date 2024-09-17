

using Fbs;
using Google.FlatBuffers;

public class UserItemNetworkHelper
{
    public static byte[] CreateGetInventoryRequestBody()
    {
        FlatBufferBuilder builder = new(1);

        RequestGetInfoInventory.StartRequestGetInfoInventory(builder);
        RequestGetInfoInventory.AddUid(builder, -1); // any number is valid, server requires non-null body;
        var offset = RequestGetInfoInventory.EndRequestGetInfoInventory(builder);

        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
