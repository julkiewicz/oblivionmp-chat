using OblivionMp.ChatMod.Common;
using OblivionMpCSharpMod.Values;

namespace OblivionMp.ChatMod;

/// <summary>A line and when it arrived here, in Unix milliseconds.</summary>
/// <remarks>
/// UNFORMATTED, because how a time reads is the page's decision and formatting it here would put a clock
/// format behind a rebuild.
/// </remarks>
internal sealed record ReceivedLine(ChatLine Line, long ArrivedAt, Color? Color = null);
