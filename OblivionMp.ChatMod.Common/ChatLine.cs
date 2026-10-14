using LiteNetLib.Utils;
using ReadyM.Api.Idents;
using ReadyM.Api.Multiplayer.Generators;

namespace OblivionMp.ChatMod.Common;

[DeriveINetSerializable]
public partial struct ChatLine : INetSerializable
{
    public ChatLineKind Kind;
    public PlayerId Sender;
    public string Nickname;
    public string Text;

    public static ChatLine FromPlayer(PlayerId sender, string nickname, string text)
        => new() { Kind = ChatLineKind.Player, Sender = sender, Nickname = nickname, Text = text };

    public static ChatLine FromServer(ChatLineKind kind, string text)
        => new() { Kind = kind, Sender = PlayerId.Server, Nickname = string.Empty, Text = text };
}
