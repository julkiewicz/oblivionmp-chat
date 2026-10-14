using Microsoft.Extensions.Logging;
using ReadyM.Api.DI;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod.Serverside;

[ModEntry]
internal sealed partial class Mod(IDependencyContainer services, ChatCommandRegistry registry, ILogger logger)
{
    private void Init()
    {
        // The handlers' own instance, so a mod that depends on chat sends through the one the clients talk to
        services.RegisterSingleton<IChat, ChatRpcHandlers>(services.Resolve<ChatRpcHandlers>());

        logger.LogInformation("Server chat initialized with {Commands} commands.", registry.All.Count());
    }
}
