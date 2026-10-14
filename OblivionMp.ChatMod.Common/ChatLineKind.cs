namespace OblivionMp.ChatMod.Common;

public enum ChatLineKind : byte
{
    Player = 0,
    System = 1,
    CommandReply = 2,
    Rejection = 3,
    Action = 4,
    Ambient = 5,
    Ooc = 6,
    Private = 7,
}
