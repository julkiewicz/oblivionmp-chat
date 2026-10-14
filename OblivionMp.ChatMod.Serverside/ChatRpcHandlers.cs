using Microsoft.Extensions.Logging;
using OblivionMp.ChatMod.Common;
using OblivionMp.Sdk.Common.Archetypes.Mixins;
using ReadyM.Api.Idents;
using ReadyM.Relay.Server.Sdk.Rpc;
using ReadyM.SDK.Archetypes.Core;
using ReadyM.SDK.Attributes;
using ReadyM.SDK.Server.Entities;

namespace OblivionMp.ChatMod.Serverside;

[RpcHandlersFor(typeof(ChatRpcContracts))]
internal sealed partial class ChatRpcHandlers(
    IEntities entities,
    ChatCommandRegistry commands,
    ILogger logger
) : IChat
{
    public IReadOnlyList<ChatPlayer> ConnectedPlayers
    {
        get
        {
            var found = new List<ChatPlayer>();

            foreach (var player in entities.Query<Player>())
            {
                found.Add(new ChatPlayer(player.PlayerId, player.Nickname.ToString()));
            }

            return found;
        }
    }

    public string GetNickname(PlayerId player)
        => ConnectedPlayers.FirstOrDefault(p => p.Id == player)?.Nickname ?? string.Empty;

    public void Broadcast(string text, ChatLineKind kind = ChatLineKind.System) => Deliver(ChatLine.FromServer(kind, text));

    public void SendTo(PlayerId player, string text, ChatLineKind kind = ChatLineKind.System)
        => SendDeliverMessage(player, ChatLine.FromServer(kind, text));

    public void AddCommand(string name, string usage, ChatCommandHandler run)
        => commands.Add(new ChatCommand { Name = name, Usage = usage, Run = run });

    partial void OnSubmitMessage(RpcContext context, string text)
    {
        var verdict = ChatGate.Review(text, out var cleaned);

        switch (verdict)
        {
            case ChatSubmissionVerdict.Message:
                Deliver(ChatLine.FromPlayer(context.Sender, GetNickname(context.Sender), cleaned));
                break;

            case ChatSubmissionVerdict.Command:
                RunCommand(context.Sender, cleaned);
                break;

            case ChatSubmissionVerdict.Empty:
                break;

            case ChatSubmissionVerdict.TooLong:
                logger.LogWarning("{Sender} submitted {Length} characters, over the {Limit} limit.",
                    context.Sender, cleaned.Length, ChatLimits.MaxMessageLength);
                SendTo(context.Sender, $"That is longer than {ChatLimits.MaxMessageLength} characters.",
                    ChatLineKind.Rejection);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(verdict), verdict, "Unhandled chat verdict.");
        }
    }

    private void Deliver(ChatLine line)
    {
        foreach (var player in ConnectedPlayers) SendDeliverMessage(player.Id, line);
    }

    private void RunCommand(PlayerId sender, string cleaned)
    {
        if (!ChatCommandRegistry.TryParse(cleaned, out var name, out var arguments))
        {
            SendTo(sender, "That is not a command.", ChatLineKind.Rejection);
            return;
        }

        if (!commands.TryGet(name, out var command))
        {
            SendTo(sender, $"There is no /{name}. Try /help.", ChatLineKind.Rejection);
            return;
        }

        try
        {
            command.Run(this, sender, arguments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "/{Command} threw while answering {Sender}.", name, sender);
            SendTo(sender, $"/{name} failed.", ChatLineKind.Rejection);
        }
    }
}