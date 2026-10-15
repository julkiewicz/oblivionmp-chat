using OblivionMp.ChatMod.Common;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod;

[RpcHandlersFor(typeof(ChatRpcContracts))]
internal partial class ChatRpcHandlers(ChatPageMessages pages)
{
    /// <summary>Raised after a line from the server is shown.</summary>
    public event Action<ChatLine>? Delivered;

    public void Submit(string text) => SendSubmitMessage(text);

    partial void OnDeliverMessage(ChatLine line)
    {
        pages.Add(line);
        Delivered?.Invoke(line);
    }
}
