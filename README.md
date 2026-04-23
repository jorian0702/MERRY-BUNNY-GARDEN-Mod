# MERRY BUNNY GARDEN Cheat Mod

BepInEx + Harmony cheat mod for MERRY BUNNY GARDEN (HEBEREKE BUNNY GARDEN).

## Features

| Hotkey | Function |
|--------|----------|
| **F1** | Full Unlock (all cheats at once) |
| **F2** | Max All Panties (108 types, count = 99) |
| **F3** | All Episodes Cleared / S-Rank |
| **F4** | Gallery / CG / Achievements Unlocked |
| **F5** | All Text Read + Tutorials Done |
| **F6** | All Costumes Unlocked |
| **F10** | Toggle Help Overlay |

### Detailed Breakdown

**Full Unlock (F1)** calls the game's built-in `UnlockAll()` on both `GameData` and `SystemData`, then applies every other cheat below.

**Max Panties (F2)** sets all 108 panty types (`Pi001_01` ~ `Pi030_02`) to count 99.

**All S-Rank (F3)** marks all 9 stages (`Stage01` ~ `Stage09`) as Cleared with S-rank on both Normal and Hard difficulty, with a best time of 30.0s.

**Gallery Unlock (F4)** unlocks all Event CGs, Minigame entries, and all 20 achievements (AfterClosing, NightRoutine, GrandEnding, PantsMaster, character endings, etc.).

**Text Read (F5)** marks all dialogue as read and all tutorials as completed.

**Costumes (F6)** unlocks all costumes for all characters (Kana, Rin, Miuka): Uniform, Swimwear, Casual, Babydoll, Shirt, BunnyGirl, Topless, GoldenBunny.

## Requirements

- .NET SDK 6.0+ (for building)
- MERRY BUNNY GARDEN (Steam version)

## Quick Setup

```powershell
cd "d:\SteamLibrary\steamapps\common\MERRY BUNNY GARDEN\ModTools"
.\setup_bepinex.ps1
```

This script will:
1. Download and install BepInEx 5.4.23.2
2. Build the cheat mod
3. Copy it to `BepInEx\plugins\`

## Manual Setup

### 1. Install BepInEx

Download [BepInEx 5.4.23.2 (x64)](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) and extract to the game root folder.

### 2. Build

```powershell
cd "d:\SteamLibrary\steamapps\common\MERRY BUNNY GARDEN\ModTools\CheatMod"
dotnet build -c Release
```

### 3. Install

Copy `ModTools\CheatMod\bin\Release\netstandard2.1\MerryBunnyCheat.dll` to `BepInEx\plugins\`.

### 4. Play

Launch the game. Press **F10** for the help overlay.

## Technical Details

- **Engine:** Unity 6 (6000.0.31f1), Mono
- **Mod Loader:** BepInEx 5.4.x + Harmony 2.x
- **Save Location:** `%USERPROFILE%\AppData\LocalLow\Qureate\HEBEREKE BUNNY GARDEN\Save\`
- **Target Assembly:** `Assembly-CSharp.dll`
- **Namespaces:** `GB.Game.GameData`, `GB.Game.SystemData`, `GB.Save.Saves`
