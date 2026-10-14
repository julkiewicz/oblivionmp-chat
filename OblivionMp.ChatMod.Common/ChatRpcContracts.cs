using ReadyM.Api.Multiplayer;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod.Common;

[RpcContracts]
public static partial class ChatRpcContracts
{
    [ClientToServer]
    public static partial void SubmitMessage(string text);

    [ServerToClient]
    public static partial void DeliverMessage(ChatLine line);
}