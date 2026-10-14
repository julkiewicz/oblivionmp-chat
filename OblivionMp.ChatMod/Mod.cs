using ReadyM.Api.DI;
using ReadyM.Cef.Rpc;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod;

[ModEntry]
internal sealed partial class Mod(IDependencyContainer services)
{
    private void Init()
    {
        services.RegisterSingleton<ChatPageMessages>();
        services.RegisterSingleton<ICefRpcHandler, ChatPageMessages>(services.Resolve<ChatPageMessages>());

        // The overlay's own instance, so a mod that depends on chat talks to the one on screen
        services.RegisterSingleton<IChat, ChatOverlay>(services.Resolve<ChatOverlay>());
    }
}