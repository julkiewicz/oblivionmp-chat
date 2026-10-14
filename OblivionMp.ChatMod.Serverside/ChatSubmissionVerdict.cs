namespace OblivionMp.ChatMod.Serverside;

internal enum ChatSubmissionVerdict : byte
{
    Message = 0,
    Command = 1,
    Empty = 2,
    TooLong = 3,
}
