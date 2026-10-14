#!powershell.exe -ExecutionPolicy Bypass -File

# The file lists MakeModFolder.ps1 uses to build
#
#   Output/mods/<modName>/manifest.json
#   Output/mods/<modName>/client/...
#   Output/mods/<modName>/server/...
#
# The server hands the client folder to connecting players and keeps the server folder to itself.

# Folder name of the packaged mod.
$modName = "OblivionMP.Chat"

# The projects the two halves build from.
$clientProject = "OblivionMp.ChatMod"
$serverProject = "OblivionMp.ChatMod.Serverside"

# --- Client half, from <clientProject>/bin/<Configuration>/net10.0 ---
$clientBuildFiles = @(
    "OblivionMp.ChatMod.dll",
    "OblivionMp.ChatMod.Common.dll",
    "web"   # the overlay's page, served to the game's browser from next to the assembly
)

# --- Server half, from <serverProject>/bin/<Configuration>/net10.0 ---
$serverBuildFiles = @(
    "OblivionMp.ChatMod.Serverside.dll",
    "OblivionMp.ChatMod.Common.dll"
)

# Copied from "Content" into the mod root, alongside the two halves.
$manifestFiles = @(
    "manifest.json"
)

# Copied from "Content" into client/ and server/.
$clientContentFiles = @()

$serverContentFiles = @()

# Added only in Debug builds.
$clientDebugBuildFiles = @(
    "OblivionMp.ChatMod.pdb",
    "OblivionMp.ChatMod.Common.pdb"
)

$serverDebugBuildFiles = @(
    "OblivionMp.ChatMod.Serverside.pdb",
    "OblivionMp.ChatMod.Common.pdb"
)
