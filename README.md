# MERRY BUNNY GARDEN - Cheat Mod

MERRY BUNNY GARDEN (HEBEREKE BUNNY GARDEN) 用の非公式 BepInEx チートプラグインです。

## 機能

| キー | 機能 |
|------|------|
| F1 | フルアンロック (以下全部を一括実行) |
| F2 | 全パンツ所持数MAX (99個) |
| F3 | 全エピソード Sランククリア済み |
| F4 | ギャラリー / CG / 実績解放 |
| F5 | 全テキスト既読 / チュートリアル完了 |
| F6 | 全コスチューム解放 |
| F7 | Steam実績全解除 |
| F10 | ヘルプ表示切替 |

## 技術スタック

- C# / .NET Standard 2.1
- [BepInEx 5](https://github.com/BepInEx/BepInEx) (Unity Mono)
- [HarmonyX](https://github.com/BepInEx/HarmonyX) によるランタイムパッチ
- Unity IMGUI によるオーバーレイ UI
- リフレクションによるセーブデータ直接操作

## ビルド

```bash
dotnet build ModTools/CheatMod/CheatMod.csproj -c Release
```

出力先: `ModTools/CheatMod/bin/Release/netstandard2.1/MerryBunnyCheat.dll`

## インストール

1. [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) をゲームディレクトリに導入
2. ビルドした `MerryBunnyCheat.dll` を `BepInEx/plugins/` に配置
3. ゲームを起動し、F10 でヘルプを確認

## ダウンロード

[Releases](../../releases) からビルド済み DLL をダウンロードできます。

## 免責事項

本プロジェクトは非公式のファンメイドMODであり、ゲーム開発元とは一切関係ありません。自己責任でご利用ください。
