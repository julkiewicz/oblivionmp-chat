using ReadyM.Api.DI;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod.Serverside;

[ModEntry]
internal sealed partial class Mod(IDependencyContainer services)
{
    private void Init()
    {
        // The handlers' own instance, so a mod that depends on chat sends through the one the clients talk to
        services.RegisterSingleton<IChat, ChatRpcHandlers>(services.Resolve<ChatRpcHandlers>());
    }
}
