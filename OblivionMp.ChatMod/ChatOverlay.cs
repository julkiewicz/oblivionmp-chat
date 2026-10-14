using Microsoft.Extensions.Logging;
using OblivionMp.ChatMod.Common;
using OblivionMp.Sdk;
using OblivionMpCSharpMod.Values;
using ReadyM.Cef;
using ReadyM.SDK.Attributes;
using ReadyM.Sdk.Common.Input;

namespace OblivionMp.ChatMod;

[Service]
internal sealed partial class ChatOverlay(
    CefHost cef,
    ChatRpcHandlers rpc,
    ChatPageMessages pages,
    IInput input,
    ILogger logger
) : IChat
{
    private const string Host = "chat";
    private const float BarHeight = 320.0f;

    private ViewportHandle _viewport;
    private BrowserHandle _browser;
    private bool _created;

    public bool IsOpen => pages.IsOpen;

    public event Action<ChatLine>? LineReceived
    {
        add => rpc.Delivered += value;
        remove => rpc.Delivered -= value;
    }

    public void Send(string text) => rpc.Submit(text);

    public void ShowLocal(string text, ChatLineKind kind = ChatLineKind.System)
        => pages.Add(ChatLine.FromServer(kind, text));

    public void ShowLocal(string text, Color color)
        => pages.Add(ChatLine.FromServer(ChatLineKind.System, text), color);

    private void Start()
    {
        pages.Submitted = rpc.Submit;
        pages.CloseRequested = Close;

        // A LINE ARRIVING IS ENOUGH TO BUILD THE OVERLAY. Chat that only appears once you have asked for it
        // cannot show you the message that made you want it.
        pages.LineArrived = EnsureCreated;
        pages.PageLoaded = OnPageLoaded;

        // CONTENT IS CLAIMED WHILE MODS INITIALIZE, never from the keybind below. The configuration is frozen
        // once every mod has started, because CEF's threads read it from then on, and registering afterwards
        // throws where a throw takes the game down.
        var directory = Path.GetDirectoryName(typeof(ChatOverlay).Assembly.Location)!;
        cef.RegisterFolder(Host, Path.Combine(directory, "web"));

        // FROM THE ENGINE'S KEY DOWN, and only a press nothing in the UI wanted. A polled bind cannot say it
        // took the press, so the same K would also be delivered to the game and typed into the entry line it
        // had just opened.
        input.RegisterKeyBind(Key.K, Open, KeySource.UnhandledByUi);

        logger.LogInformation("[Chat] Registered: K opens chat, Enter sends, Escape puts it away.");
    }

    /// <summary>Shows the entry line and gives it the keyboard.</summary>
    /// <remarks>
    /// THE KEYBOARD ONLY, and nothing else of the game's. Chat takes Slate's focus, which is what routes keys
    /// to the page, and leaves the game's input mode alone: the mouse keeps doing what it was doing, so you
    /// can still look around while typing and no cursor appears.
    /// <para>
    /// Handing input to the UI is what used to draw one. It releases the mouse, and a viewport with neither
    /// capture nor focus gets the default cursor from the engine without the player controller's
    /// "no cursor" ever being read.
    /// </para>
    /// </remarks>
    public void Open()
    {
        EnsureCreated();

        if (pages.IsOpen) return;

        cef.SetFocused(_viewport, true);
        pages.SetOpen(true);
    }

    /// <summary>
    /// Puts the entry line away and hands input back to the game. The overlay stays on screen: the log fades
    /// on its own, so a message that arrives while you are playing is still readable.
    /// </summary>
    public void Close()
    {
        if (!pages.IsOpen) return;

        pages.SetOpen(false);
        cef.SetFocused(_viewport, false);
    }

    /// <summary>
    /// A page has loaded, and a reload is indistinguishable from a first load here.
    /// </summary>
    /// <remarks>
    /// A NEW DOCUMENT STARTS UNFOCUSED however the engine's focus has not moved, so nothing tells the browser
    /// it still holds the keyboard. Until it is told, it dispatches no keys into the page at all, and the
    /// page's own entry line can be focused and still hear nothing, Escape included.
    /// </remarks>
    private void OnPageLoaded()
    {
        if (!pages.IsOpen) return;

        cef.SetPageFocused(_browser, true);
    }

    private void EnsureCreated()
    {
        if (_created) return;
        _created = true;

        _viewport = cef.CreateViewport(zOrder: 0);
        cef.SetPlacement(_viewport, ViewportPlacement.HorizontalBar(ViewportAlign.End, BarHeight));

        var browser = cef.CreateBrowser();
        _browser = browser.Handle;
        pages.Page = browser.Handle;

        browser.Navigate($"readym://{Host}/");
        cef.Assign(_viewport, browser.Handle);

        // VISIBLE FROM HERE ON. The page draws nothing until it has something to say and Chromium paints only
        // on damage, so an idle overlay costs a texture and no frames.
        cef.SetVisible(_viewport, true);

        logger.LogInformation("[Chat] Ready on readym://{Host}/.", Host);
    }
}
