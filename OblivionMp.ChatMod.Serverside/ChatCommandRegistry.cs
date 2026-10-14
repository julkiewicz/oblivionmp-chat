using ReadyM.SDK.Attributes;

namespace OblivionMp.ChatMod.Serverside;

[Service]
internal sealed partial class ChatCommandRegistry
{
    private readonly Dictionary<string, ChatCommand> _commands = new(StringComparer.OrdinalIgnoreCase);

    public void Add(ChatCommand command)
    {
        if (!_commands.TryAdd(command.Name, command))
            throw new ArgumentException($"/{command.Name} is already registered.", nameof(command));
    }

    public bool TryGet(string name, out ChatCommand command) => _commands.TryGetValue(name, out command);

    public IEnumerable<ChatCommand> All => _commands.Values.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);

    public static bool TryParse(string text, out string name, out string arguments)
    {
        name = string.Empty;
        arguments = string.Empty;

        if (text.Length < 2 || text[0] != '/') return false;

        var body = text[1..];
        var space = body.IndexOf(' ');

        if (space < 0)
        {
            name = body;
            return true;
        }

        name = body[..space];
        arguments = body[(space + 1)..].Trim();
        return name.Length > 0;
    }
}
