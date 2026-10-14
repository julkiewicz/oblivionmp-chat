namespace OblivionMp.ChatMod.Serverside;

internal struct ChatCommand
{
    public string Name;
    public string Usage;
    public ChatCommandHandler Run;
}
