using OblivionMp.ChatMod.Common;
using OblivionMpCSharpMod.Values;

namespace OblivionMp.ChatMod;

/// The chat overlay, for other client mods to use. Take it as a constructor parameter, like any service.
/// <remarks>
/// Your mod has to list the chat mod (uniqueId <c>ChatMod</c>) as a dependency in its manifest, so chat is loaded
/// first, and the server has to run the chat mod too.
/// </remarks>
public interface IChat
{
    /// Whether the entry line is up and has the keyboard.
    bool IsOpen { get; }

    /// Raised for every line the server delivers, after it is shown. Lines from <see cref="ShowLocal(string, ChatLineKind)"/> are not.
    event Action<ChatLine>? LineReceived;

    /// Sends text as if the player had typed it, so a leading slash runs a chat command.
    void Send(string text);

    /// Shows a line only this player sees, styled like the server's lines of that kind. Nothing goes to the server.
    void ShowLocal(string text, ChatLineKind kind = ChatLineKind.System);

    /// Shows a line only this player sees, in the given color. Nothing goes to the server.
    void ShowLocal(string text, Color color);

    /// Shows the entry line and gives it the keyboard, as pressing K does.
    void Open();

    /// Puts the entry line away and hands the keyboard back to the game. Recent lines stay up until they fade.
    void Close();
}
