using OblivionMp.ChatMod.Common;
using ReadyM.Api.Idents;

namespace OblivionMp.ChatMod.Serverside;

/// <summary>Server chat, for other server mods to use. Take it as a constructor parameter, like any service.</summary>
/// <remarks>
/// Your mod has to list the chat mod (uniqueId <c>OblivionMP.Chat</c>) as a dependency in its manifest, so chat is loaded
/// first.
/// </remarks>
public interface IChat
{
    /// <summary>The players connected right now, with the nicknames chat shows for them.</summary>
    IReadOnlyList<ChatPlayer> ConnectedPlayers { get; }

    /// <summary>The nickname chat shows for a player, or an empty string for one who is not connected.</summary>
    string GetNickname(PlayerId player);

    /// <summary>Shows a line to every connected player.</summary>
    void Broadcast(string text, ChatLineKind kind = ChatLineKind.System);

    /// <summary>Shows a line to one player.</summary>
    void SendTo(PlayerId player, string text, ChatLineKind kind = ChatLineKind.System);

    /// <summary>Adds a slash command. Names are case-insensitive and appear in /help with their usage.</summary>
    /// <exception cref="ArgumentException">A command with this name is already registered.</exception>
    void AddCommand(string name, string usage, ChatCommandHandler run);
}

/// <summary>Runs a chat command. <paramref name="arguments"/> is everything after the command name, trimmed.</summary>
public delegate void ChatCommandHandler(IChat chat, PlayerId sender, string arguments);
