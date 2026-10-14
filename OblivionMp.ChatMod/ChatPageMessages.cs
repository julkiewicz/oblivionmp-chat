using System.Text.Json;
using OblivionMp.ChatMod.Common;
using OblivionMpCSharpMod.Values;
using ReadyM.Api.DI;
using ReadyM.Cef;
using ReadyM.Cef.Rpc;

namespace OblivionMp.ChatMod;

internal sealed partial class ChatPageMessages
{
    private const int Capacity = 100;

    private readonly List<ReceivedLine> _lines = new(Capacity);

    private BrowserHandle? _page;

    public BrowserHandle Page { set => _page = value; }

    public Action<string>? Submitted { get; set; }

    public Action? CloseRequested { get; set; }

    /// <summary>Raised before a line is shown, so the overlay can exist by the time there is one to show.</summary>
    public Action? LineArrived { get; set; }

    /// <summary>Raised when a page has finished loading, which a reload looks exactly like.</summary>
    public Action? PageLoaded { get; set; }

    /// <summary>Whether the entry line is up, which is the state the page was last told to be in.</summary>
    public bool IsOpen { get; private set; }

    /// <summary>Shows or hides the page's entry line, and moves the page's own focus with it.</summary>
    /// <remarks>
    /// Remembered rather than only sent, because a page that has just loaded missed whatever it was told
    /// before and asks for the state along with its history.
    /// </remarks>
    public void SetOpen(bool open)
    {
        IsOpen = open;
        if (_page is { } page) SendOpen(page, open);
    }

    /// Shows a line, in <paramref name="color"/> when given rather than the color its kind gets.
    public void Add(ChatLine line, Color? color = null)
    {
        if (_lines.Count == Capacity)
            _lines.RemoveAt(0);

        var entry = new ReceivedLine(line, DateTimeOffset.Now.ToUnixTimeMilliseconds(), color);
        _lines.Add(entry);

        LineArrived?.Invoke();

        if (_page is { } page) 
            SendLine(page, Json([entry]));
    }

    [ReceiveFromCef]
    private void OnSubmit(BrowserHandle page, string text) => Submitted?.Invoke(text);

    [ReceiveFromCef]
    private void OnClose(BrowserHandle page) => CloseRequested?.Invoke();

    /// <summary>The first thing a page does, so it is also how the game hears that one has loaded.</summary>
    [ReceiveFromCef]
    private void OnRequestHistory(BrowserHandle page)
    {
        SendHistory(page, Json(_lines));
        SendOpen(page, IsOpen);
        PageLoaded?.Invoke();
    }

    [SendToCef]
    public partial void SendLine(BrowserHandle page, string json);

    [SendToCef]
    public partial void SendHistory(BrowserHandle page, string json);

    [SendToCef]
    public partial void SendOpen(BrowserHandle page, bool open);

    private static string Json(IReadOnlyList<ReceivedLine> lines)
        => JsonSerializer.Serialize(lines.Select(e => new
        {
            kind = e.Line.Kind.ToString(),
            nickname = e.Line.Nickname ?? string.Empty,
            text = e.Line.Text ?? string.Empty,
            at = e.ArrivedAt,
            color = e.Color is { } c ? Css(c) : null,
        }));

    private static string Css(Color c)
        => FormattableString.Invariant(
            $"rgba({Channel(c.R)}, {Channel(c.G)}, {Channel(c.B)}, {Math.Clamp(c.A, 0f, 1f):0.###})");

    private static int Channel(float value) => (int)MathF.Round(Math.Clamp(value, 0f, 1f) * 255f);
}
