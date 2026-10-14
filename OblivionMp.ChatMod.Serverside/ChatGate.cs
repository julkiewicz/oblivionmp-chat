using OblivionMp.ChatMod.Common;

namespace OblivionMp.ChatMod.Serverside;

internal static class ChatGate
{
    public static ChatSubmissionVerdict Review(string? text, out string cleaned)
    {
        cleaned = text?.Trim() ?? string.Empty;

        if (cleaned.Length == 0) return ChatSubmissionVerdict.Empty;
        if (cleaned.Length > ChatLimits.MaxMessageLength) return ChatSubmissionVerdict.TooLong;

        return cleaned[0] == '/' ? ChatSubmissionVerdict.Command : ChatSubmissionVerdict.Message;
    }
}
