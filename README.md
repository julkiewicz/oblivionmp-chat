# OblivionMP chat

Server-authoritative text chat for [OblivionMP](https://readym.io), with an in-game web overlay. Press K to
open it, Enter to send, Escape to put it away. Slash commands run on the server: `/me`, `/do`, `/b`, `/dm`,
`/who` and `/help`.

It is an optional mod. Other mods can use it through one of two NuGet packages, one per environment:

| your project | package |
|---|---|
| the mod that runs in the game | `ReadyM.Oblivion.Chat.Client` |
| the mod that runs on the relay server | `ReadyM.Oblivion.Chat.Server` |

Add the one that matches each project:

```bash
dotnet add package ReadyM.Oblivion.Chat.Client
dotnet add package ReadyM.Oblivion.Chat.Server
```

Add `--prerelease` while the version you want is a preview.

Each gives you an `IChat` to take as a constructor parameter, like any service.

```csharp
// client: a line only this player sees
chat.ShowLocal("The match starts in 30 seconds.", new Color(1, 1, 0));

// server: a line for everyone, and a command of your own
chat.Broadcast("The arena is open.");
chat.AddCommand("ready", "/ready - mark yourself ready", (c, sender, _) => c.SendTo(sender, "You are ready."));
```

A mod that uses chat has to list it as a dependency in its `manifest.json`, so chat is loaded first, and the
server has to run the chat mod too:

```json
"dependencies": [
  { "uniqueId": "OblivionMP.SDK", "minimumVersion": "1.0.0" },
  { "uniqueId": "ChatMod", "minimumVersion": "1.0.0" }
]
```

The chat assemblies are loaded by the chat mod itself, so your mod folder only needs your own.

## Building

The projects build against the OblivionMP SDK packages from NuGet: `ReadyM.SDK.Oblivion.Common`, `.Client` and
`.Server`. Their version is set once, as `OblivionSdkVersion` in `Directory.Build.props`.

```powershell
.\MakeModFolder.ps1 Release                  # the mod, into Output/mods/OblivionMp.ChatMod
.\BuildNugetPackages.ps1                     # the two packages above, into Output/nuget
```

Use `Debug` instead of `Release` to include `.pdb` symbol files.

Copy `Output/mods/OblivionMp.ChatMod/` into your server's `mods/` directory, next to the `OblivionMp.Sdk` mod,
and restart the server. Connecting players receive the client half automatically.
