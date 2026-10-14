using OblivionMp.ChatMod.Common;
using ReadyM.Api.Idents;
using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod.Serverside;

[Service]
internal sealed partial class ChatCommands(ChatCommandRegistry registry)
{
    private void Start()
    {
        registry.Add(new ChatCommand
        {
            Name = "me",
            Usage = "/me <action> - what your character is doing",
            Run = (chat, sender, arguments) =>
            {
                if (RefuseIfEmpty(chat, sender, arguments, "/me sits down by the fire")) return;

                chat.Broadcast($"* {chat.GetNickname(sender)} {arguments}", ChatLineKind.Action);
            },
        });

        registry.Add(new ChatCommand
        {
            Name = "do",
            Usage = "/do <description> - what is true about your character or the scene",
            Run = (chat, sender, arguments) =>
            {
                if (RefuseIfEmpty(chat, sender, arguments, "/do His armour is scratched and muddy.")) return;

                chat.Broadcast($"* {arguments} (( {chat.GetNickname(sender)} ))", ChatLineKind.Ambient);
            },
        });

        registry.Add(new ChatCommand
        {
            Name = "b",
            Usage = "/b <text> - local out of character chat",
            Run = (chat, sender, arguments) =>
            {
                if (RefuseIfEmpty(chat, sender, arguments, "/b back in five minutes")) return;

                chat.Broadcast($"(( {chat.GetNickname(sender)}: {arguments} ))", ChatLineKind.Ooc);
            },
        });

        registry.Add(new ChatCommand
        {
            Name = "dm",
            Usage = "/dm <id> <text> - a private message to one player",
            Run = SendPrivate,
        });

        registry.Add(new ChatCommand
        {
            Name = "who",
            Usage = "/who - who is connected, with the ids /dm needs",
            Run = (chat, sender, _) =>
            {
                var players = chat.ConnectedPlayers;
                if (players.Count == 0)
                {
                    chat.SendTo(sender, "Nobody is connected.", ChatLineKind.CommandReply);
                    return;
                }

                var names = string.Join(", ", players.Select(p => $"{p.Nickname} [{p.Id.RawValue}]"));
                chat.SendTo(sender, $"{players.Count} connected: {names}", ChatLineKind.CommandReply);
            },
        });

        registry.Add(new ChatCommand
        {
            Name = "help",
            Usage = "/help - this list",
            Run = (chat, sender, _) =>
            {
                foreach (var command in registry.All)
                    chat.SendTo(sender, command.Usage, ChatLineKind.CommandReply);
            },
        });
    }

    private static void SendPrivate(IChat chat, PlayerId sender, string arguments)
    {
        var space = arguments.IndexOf(' ');
        if (space < 0 || !ushort.TryParse(arguments[..space], out var rawId))
        {
            chat.SendTo(sender, "Usage: /dm <id> <text>. /who shows the ids.", ChatLineKind.Rejection);
            return;
        }

        var text = arguments[(space + 1)..].Trim();
        if (text.Length == 0)
        {
            chat.SendTo(sender, "Nothing to send.", ChatLineKind.Rejection);
            return;
        }

        var target = chat.ConnectedPlayers.FirstOrDefault(p => p.Id.RawValue == rawId);
        if (target is null)
        {
            chat.SendTo(sender, $"There is no player with id {rawId}.", ChatLineKind.Rejection);
            return;
        }

        var from = chat.GetNickname(sender);

        chat.SendTo(target.Id, $"PM from {from} [{sender.RawValue}]: {text}", ChatLineKind.Private);
        chat.SendTo(sender, $"PM to {target.Nickname} [{rawId}]: {text}", ChatLineKind.Private);
    }

    private static bool RefuseIfEmpty(IChat chat, PlayerId sender, string arguments, string example)
    {
        if (arguments.Length > 0) return false;

        chat.SendTo(sender, $"Nothing to say. For example: {example}", ChatLineKind.Rejection);
        return true;
    }
}
