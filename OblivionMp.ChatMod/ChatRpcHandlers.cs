using OblivionMp.ChatMod.Common;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod;

[RpcHandlersFor(typeof(ChatRpcContracts))]
internal partial class ChatRpcHandlers(ChatPageMessages pages)
{
    /// Raised after a line from the server is shown.
    public event Action<ChatLine>? Delivered;

    public void Submit(string text) => SendSubmitMessage(text);

    partial void OnDeliverMessage(ChatLine line)
    {
        pages.Add(line);
        Delivered?.Invoke(line);
    }
}
