$ErrorActionPreference = "Stop"

$gameRoot = Split-Path -Parent $PSScriptRoot
$zipUrl = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip"
$zipPath = Join-Path $PSScriptRoot "BepInEx_win_x64.zip"

Write-Host "=== BepInEx Setup for MERRY BUNNY GARDEN ===" -ForegroundColor Cyan
Write-Host "Game root: $gameRoot"

if (Test-Path (Join-Path $gameRoot "BepInEx\core\BepInEx.dll")) {
    Write-Host "BepInEx is already installed." -ForegroundColor Green
}
else {
    Write-Host "Downloading BepInEx 5.4.23.2..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath -UseBasicParsing
    Write-Host "Extracting to game root..." -ForegroundColor Yellow
    Expand-Archive -Path $zipPath -DestinationPath $gameRoot -Force
    Write-Host "BepInEx installed!" -ForegroundColor Green
}

$pluginsDir = Join-Path $gameRoot "BepInEx\plugins"
if (-not (Test-Path $pluginsDir)) {
    New-Item -ItemType Directory -Path $pluginsDir -Force | Out-Null
}

Write-Host ""
Write-Host "=== Building CheatMod ===" -ForegroundColor Cyan

$projDir = Join-Path $PSScriptRoot "CheatMod"
Push-Location $projDir
try {
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    $dll = Join-Path $projDir "bin\Release\netstandard2.1\MerryBunnyCheat.dll"
    if (Test-Path $dll) {
        Copy-Item $dll $pluginsDir -Force
        Write-Host "MerryBunnyCheat.dll -> BepInEx\plugins\" -ForegroundColor Green
    }
    else {
        throw "DLL not found at $dll"
    }
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Green
Write-Host "Launch the game to activate the cheat mod."
Write-Host "Press F10 in-game to see the help overlay."
