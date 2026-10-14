#!powershell.exe -ExecutionPolicy Bypass -File

<#
    Builds the chat mod's two NuGet packages, for other mods to build against:

      ReadyM.Oblivion.Chat.Client   IChat for a client mod
      ReadyM.Oblivion.Chat.Server   IChat for a server mod

    Each carries the shared chat types as well. The mod itself is packaged by MakeModFolder.ps1.

      -PackageVersion   override the version from Directory.Build.props, e.g. 1.0.0-rc.2
#>

param(
    [string] $Configuration = 'Release',
    [string] $Output = (Join-Path $PSScriptRoot 'Output\nuget'),
    [string] $PackageVersion
)

$ErrorActionPreference = 'Stop'

$projects = @(
    'OblivionMp.ChatMod\OblivionMp.ChatMod.csproj'
    'OblivionMp.ChatMod.Serverside\OblivionMp.ChatMod.Serverside.csproj'
)

$Output = [System.IO.Path]::GetFullPath($Output)
New-Item -ItemType Directory -Force $Output | Out-Null

$packArgs = @()
if ($PackageVersion)
{
    $packArgs += @("-p:PackageVersion=$PackageVersion", "-p:Version=$PackageVersion")
}

foreach ($project in $projects)
{
    Write-Output ''
    Write-Output "Packing $project ($Configuration)"
    & dotnet pack (Join-Path $PSScriptRoot $project) -c $Configuration -o $Output @packArgs
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "pack failed for $project"
        exit 1
    }
}

Write-Output ''
Write-Output "Built into $Output"
Get-ChildItem $Output -Filter 'ReadyM.Oblivion.Chat.*.nupkg' |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 2 |
        ForEach-Object { Write-Output "  $($_.Name)" }
