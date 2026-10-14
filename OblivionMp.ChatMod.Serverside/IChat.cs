using OblivionMp.ChatMod.Common;
using ReadyM.Api.Idents;

namespace OblivionMp.ChatMod.Serverside;

/// Server chat, for other server mods to use. Take it as a constructor parameter, like any service.
/// <remarks>
/// Your mod has to list the chat mod (uniqueId <c>OblivionMP.Chat</c>) as a dependency in its manifest, so chat is loaded
/// first.
/// </remarks>
public interface IChat
{
    /// The players connected right now, with the nicknames chat shows for them.
    IReadOnlyList<ChatPlayer> ConnectedPlayers { get; }

    /// The nickname chat shows for a player, or an empty string for one who is not connected.
    string GetNickname(PlayerId player);

    /// Shows a line to every connected player.
    void Broadcast(string text, ChatLineKind kind = ChatLineKind.System);

    /// Shows a line to one player.
    void SendTo(PlayerId player, string text, ChatLineKind kind = ChatLineKind.System);

    /// Adds a slash command. Names are case-insensitive and appear in /help with their usage.
    /// <exception cref="ArgumentException">A command with this name is already registered.</exception>
    void AddCommand(string name, string usage, ChatCommandHandler run);
}

/// Runs a chat command. <paramref name="arguments"/> is everything after the command name, trimmed.
public delegate void ChatCommandHandler(IChat chat, PlayerId sender, string arguments);
