using OblivionMp.ChatMod.Common;
using OblivionMpCSharpMod.Values;

namespace OblivionMp.ChatMod;

/// <summary>
/// The chat overlay, for other client mods to use. Take it as a constructor parameter, like any service.
/// </summary>
/// <remarks>
/// Your mod has to list the chat mod (uniqueId <c>OblivionMP.Chat</c>) as a dependency in its manifest, so chat is loaded
/// first, and the server has to run the chat mod too.
/// </remarks>
public interface IChat
{
    /// <summary>Whether the entry line is up and has the keyboard.</summary>
    bool IsOpen { get; }

    /// <summary>
    /// Raised for every line the server delivers, after it is shown. Lines from <see cref="ShowLocal(string, ChatLineKind)"/> are not.
    /// </summary>
    event Action<ChatLine>? LineReceived;

    /// <summary>Sends text as if the player had typed it, so a leading slash runs a chat command.</summary>
    void Send(string text);

    /// <summary>
    /// Shows a line only this player sees, styled like the server's lines of that kind. Nothing goes to the server.
    /// </summary>
    void ShowLocal(string text, ChatLineKind kind = ChatLineKind.System);

    /// <summary>Shows a line only this player sees, in the given color. Nothing goes to the server.</summary>
    void ShowLocal(string text, Color color);

    /// <summary>Shows the entry line and gives it the keyboard, as pressing K does.</summary>
    void Open();

    /// <summary>
    /// Puts the entry line away and hands the keyboard back to the game. Recent lines stay up until they fade.
    /// </summary>
    void Close();
}
