using UnityEngine;
using UnityEngine.InputSystem;
using GB.Save;
using GB.Game;
using HarmonyLib;
using Steamworks;
using System;
using System.Reflection;

namespace MerryBunnyCheat
{
    public class CheatController : MonoBehaviour
    {
        internal static Saves SavesRef;
        private bool _showHelp;
        private float _msgTimer;
        private string _lastMsg = "";
        private float _findTimer;
        private GUIStyle _titleStyle;
        private GUIStyle _textStyle;
        private GUIStyle _msgStyle;

        private static bool KeyPressed(Key key)
        {
            var kb = Keyboard.current;
            if (kb == null) return false;
            return kb[key].wasPressedThisFrame;
        }

        private void Update()
        {
            if (SavesRef == null)
            {
                _findTimer -= Time.unscaledDeltaTime;
                if (_findTimer <= 0f)
                {
                    _findTimer = 5f;
                    Plugin.Log.LogInfo("[Cheat] Waiting for save data...");
                }
            }

            if (KeyPressed(Key.F1))  { ApplyFullUnlock();       ShowMsg("FULL UNLOCK!"); }
            if (KeyPressed(Key.F2))  { ApplyMaxPanties();       ShowMsg("ALL PANTIES MAX (99)!"); }
            if (KeyPressed(Key.F3))  { ApplyAllSRank();         ShowMsg("ALL EPISODES S-RANK!"); }
            if (KeyPressed(Key.F4))  { ApplyGalleryUnlock();    ShowMsg("GALLERY / CG / ACHIEVEMENTS UNLOCKED!"); }
            if (KeyPressed(Key.F5))  { ApplyTextRead();         ShowMsg("ALL TEXT READ / TUTORIALS DONE!"); }
            if (KeyPressed(Key.F6))  { ApplyUnlockCostumes();   ShowMsg("ALL COSTUMES UNLOCKED!"); }
            if (KeyPressed(Key.F7))  { UnlockSteamAchievements(); ShowMsg("STEAM ACHIEVEMENTS UNLOCKED!"); }
            if (KeyPressed(Key.F10)) { _showHelp = !_showHelp; }

            if (_msgTimer > 0f) _msgTimer -= Time.unscaledDeltaTime;
        }

        private void OnGUI()
        {
            InitStyles();

            if (_showHelp)
            {
                GUI.Box(new Rect(8, 8, 440, 260), "");
                GUI.Label(new Rect(18, 12, 420, 30), "Merry Bunny Cheat", _titleStyle);

                float y = 48;
                Line(ref y, "F1",  "Full Unlock (all cheats at once)");
                Line(ref y, "F2",  "Max All Panties (count = 99)");
                Line(ref y, "F3",  "All Episodes Cleared / S-Rank");
                Line(ref y, "F4",  "Gallery / CG / Achievements");
                Line(ref y, "F5",  "All Text Read + Tutorials Done");
                Line(ref y, "F6",  "All Costumes Unlocked");
                Line(ref y, "F7",  "Unlock Steam Achievements");
                Line(ref y, "F10", "Toggle This Help");

                string status = SavesRef != null ? "Save: OK" : "Save: not loaded yet";
                _textStyle.normal.textColor = SavesRef != null ? Color.green : Color.red;
                GUI.Label(new Rect(18, y + 4, 420, 22), status, _textStyle);
                _textStyle.normal.textColor = Color.white;
            }

            if (_msgTimer > 0f)
            {
                float alpha = Mathf.Clamp01(_msgTimer);
                var c = _msgStyle.normal.textColor;
                c.a = alpha;
                _msgStyle.normal.textColor = c;
                var size = _msgStyle.CalcSize(new GUIContent(_lastMsg));
                GUI.Label(new Rect((Screen.width - size.x) / 2f, 40, size.x + 20, 36), _lastMsg, _msgStyle);
            }
        }

        private void Line(ref float y, string key, string desc)
        {
            _textStyle.normal.textColor = Color.cyan;
            GUI.Label(new Rect(18, y, 50, 22), key, _textStyle);
            _textStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(72, y, 380, 22), desc, _textStyle);
            y += 24;
        }

        private void InitStyles()
        {
            if (_titleStyle != null) return;
            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold
            };
            _titleStyle.normal.textColor = Color.yellow;

            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            _textStyle.normal.textColor = Color.white;

            _msgStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _msgStyle.normal.textColor = Color.green;
        }

        private void ShowMsg(string msg) { _lastMsg = msg; _msgTimer = 3f; }

        private GameData GetGameData()
        {
            try { return SavesRef?.GameData; }
            catch { return null; }
        }

        private SystemData GetSystemData()
        {
            try { return SavesRef?.SystemData; }
            catch { return null; }
        }

        // ─── F1: Full Unlock ───────────────────────────────────────
        private void ApplyFullUnlock()
        {
            var gd = GetGameData();
            var sd = GetSystemData();
            if (gd == null || sd == null)
            {
                Plugin.Log.LogWarning("[Cheat] Save data not loaded yet!");
                ShowMsg("Save data not loaded!");
                return;
            }

            try { gd.UnlockAll(); } catch (Exception e) { Plugin.Log.LogWarning($"[Cheat] GameData.UnlockAll: {e.Message}"); }
            try { sd.UnlockAll(); } catch (Exception e) { Plugin.Log.LogWarning($"[Cheat] SystemData.UnlockAll: {e.Message}"); }

            ApplyUnlockCostumes();
            ApplyMaxPanties();
            ApplyAllSRank();
            ApplyGalleryUnlock();
            ApplyTextRead();
            UnlockSteamAchievements();

            Plugin.Log.LogInfo("[Cheat] === FULL UNLOCK COMPLETE ===");
        }

        // ─── F2: Max Panties ──────────────────────────────────────
        private void ApplyMaxPanties()
        {
            var gd = GetGameData();
            if (gd == null) return;

            try
            {
                var pantiesField = AccessTools.Field(typeof(GameData), "_panties");
                var panties = pantiesField.GetValue(gd) as Array;
                if (panties == null) { Plugin.Log.LogWarning("[Cheat] _panties is null"); return; }

                var elemType = panties.GetType().GetElementType();
                var countField = AccessTools.Field(elemType, "Count");
                var isNewField = AccessTools.Field(elemType, "IsNew");

                for (int i = 0; i < panties.Length; i++)
                {
                    var item = panties.GetValue(i);
                    countField.SetValue(item, 99);
                    if (isNewField != null) isNewField.SetValue(item, false);
                    panties.SetValue(item, i);
                }

                Plugin.Log.LogInfo($"[Cheat] {panties.Length} panties maxed to 99");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] MaxPanties: {e}"); }
        }

        // ─── F3: All Episodes S-Rank ──────────────────────────────
        private void ApplyAllSRank()
        {
            var gd = GetGameData();
            if (gd == null) return;

            try
            {
                var episodesField = AccessTools.Field(typeof(GameData), "_episodes");
                var episodes = episodesField.GetValue(gd) as Array;
                if (episodes == null) return;

                var epType = episodes.GetType().GetElementType();
                var statesField = AccessTools.Field(epType, "States");
                var statsField = AccessTools.Field(epType, "StageStats");
                var notifiedField = AccessTools.Field(epType, "IsUnlockNotified");

                for (int i = 0; i < episodes.Length; i++)
                {
                    var ep = episodes.GetValue(i);
                    if (notifiedField != null) notifiedField.SetValue(ep, true);

                    var states = statesField?.GetValue(ep) as Array;
                    if (states != null)
                    {
                        var psType = states.GetType().GetElementType();
                        object cleared = Enum.ToObject(psType, 4); // PlayStates.Cleared
                        for (int j = 0; j < states.Length; j++)
                            states.SetValue(cleared, j);
                    }

                    var stats = statsField?.GetValue(ep) as Array;
                    if (stats != null)
                    {
                        var stType = stats.GetType().GetElementType();
                        var bestTimeF = AccessTools.Field(stType, "BestTime");
                        var rankF = AccessTools.Field(stType, "ClearRank");

                        for (int j = 0; j < stats.Length; j++)
                        {
                            var stat = stats.GetValue(j);
                            bestTimeF?.SetValue(stat, 30.0f);

                            if (rankF != null)
                                SetFieldSmart(stat, rankF, 4);

                            stats.SetValue(stat, j);
                        }
                    }

                    episodes.SetValue(ep, i);
                }

                Plugin.Log.LogInfo($"[Cheat] {episodes.Length} episodes set to S-rank cleared");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] AllSRank: {e}"); }
        }

        // ─── F4: Gallery / CG / Achievements ──────────────────────
        private void ApplyGalleryUnlock()
        {
            var sd = GetSystemData();
            if (sd == null) return;

            try
            {
                SetAllUnlockStates(sd, "EventCgStates");
                SetAllUnlockStates(sd, "MinigameStates");
                SetAllBoolFlags(sd, "AchievementFlags");

                Plugin.Log.LogInfo("[Cheat] Gallery / CG / Achievements unlocked");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] GalleryUnlock: {e}"); }
        }

        // ─── F5: All Text Read + Tutorials ─────────────────────────
        private void ApplyTextRead()
        {
            var sd = GetSystemData();
            if (sd == null) return;

            try
            {
                SetAllBoolFlags(sd, "TextReadFlags");
                SetAllBoolFlags(sd, "TutorialFlags");

                Plugin.Log.LogInfo("[Cheat] All text read / tutorials done");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] TextRead: {e}"); }
        }

        // ─── F6: All Costumes ──────────────────────────────────────
        private void ApplyUnlockCostumes()
        {
            var gd = GetGameData();
            if (gd == null) return;

            try
            {
                var charasField = AccessTools.Field(typeof(GameData), "_charas");
                var charas = charasField.GetValue(gd) as Array;
                if (charas == null) return;

                var cType = charas.GetType().GetElementType();
                var costumesField = AccessTools.Field(cType, "Costumes");
                var cNotified = AccessTools.Field(cType, "IsUnlockNotified");

                for (int i = 0; i < charas.Length; i++)
                {
                    var chara = charas.GetValue(i);
                    cNotified?.SetValue(chara, true);

                    var costumes = costumesField?.GetValue(chara) as Array;
                    if (costumes != null)
                    {
                        var cosType = costumes.GetType().GetElementType();
                        var stateF = AccessTools.Field(cosType, "State");
                        var cosNot = AccessTools.Field(cosType, "IsUnlockNotified");

                        for (int j = 0; j < costumes.Length; j++)
                        {
                            var costume = costumes.GetValue(j);
                            if (stateF != null)
                                SetFieldSmart(costume, stateF, 2); // Unlocked
                            cosNot?.SetValue(costume, true);
                            costumes.SetValue(costume, j);
                        }
                    }

                    charas.SetValue(chara, i);
                }

                Plugin.Log.LogInfo("[Cheat] All costumes unlocked");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] Costumes: {e}"); }
        }

        // ─── F7: Steam Achievements ───────────────────────────────
        private void UnlockSteamAchievements()
        {
            try
            {
                if (!SteamAPI.IsSteamRunning())
                {
                    Plugin.Log.LogWarning("[Cheat] Steam is not running");
                    ShowMsg("Steam is not running!");
                    return;
                }

                SteamUserStats.RequestCurrentStats();

                uint count = SteamUserStats.GetNumAchievements();
                int unlocked = 0;
                for (uint i = 0; i < count; i++)
                {
                    string name = SteamUserStats.GetAchievementName(i);
                    if (!string.IsNullOrEmpty(name))
                    {
                        bool alreadyUnlocked;
                        SteamUserStats.GetAchievement(name, out alreadyUnlocked);
                        if (!alreadyUnlocked)
                        {
                            SteamUserStats.SetAchievement(name);
                            unlocked++;
                        }
                    }
                }

                SteamUserStats.StoreStats();
                Plugin.Log.LogInfo($"[Cheat] Steam: {unlocked} new achievements unlocked (total: {count})");
            }
            catch (Exception e) { Plugin.Log.LogError($"[Cheat] SteamAchievements: {e}"); }
        }

        // ─── Utility ──────────────────────────────────────────────

        private static void SetFieldSmart(object target, FieldInfo field, int value)
        {
            var ft = field.FieldType;

            if (ft.IsEnum)
            {
                field.SetValue(target, Enum.ToObject(ft, value));
                return;
            }

            if (ft.IsValueType && !ft.IsPrimitive)
            {
                var instance = Activator.CreateInstance(ft);
                var innerFields = ft.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var f in innerFields)
                {
                    if (f.FieldType.IsEnum)
                    {
                        f.SetValue(instance, Enum.ToObject(f.FieldType, value));
                        field.SetValue(target, instance);
                        return;
                    }
                    if (f.FieldType == typeof(int))
                    {
                        f.SetValue(instance, value);
                        field.SetValue(target, instance);
                        return;
                    }
                    if (f.FieldType == typeof(byte))
                    {
                        f.SetValue(instance, (byte)value);
                        field.SetValue(target, instance);
                        return;
                    }
                }

                Plugin.Log.LogWarning($"[Cheat] SetFieldSmart: could not set {field.Name} (type {ft.FullName}), inner fields: {string.Join(", ", Array.ConvertAll(innerFields, f => $"{f.FieldType.Name} {f.Name}"))}");
                return;
            }

            field.SetValue(target, value);
        }

        private static void SetAllUnlockStates(SystemData sd, string fieldName)
        {
            var field = AccessTools.Field(typeof(SystemData), fieldName);
            var arr = field?.GetValue(sd) as Array;
            if (arr == null) return;

            var elemType = arr.GetType().GetElementType();

            if (elemType.IsEnum)
            {
                object unlocked = Enum.ToObject(elemType, 2);
                for (int i = 0; i < arr.Length; i++)
                    arr.SetValue(unlocked, i);
            }
            else if (elemType.IsValueType)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var instance = Activator.CreateInstance(elemType);
                    var innerFields = elemType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var f in innerFields)
                    {
                        if (f.FieldType.IsEnum)
                        {
                            f.SetValue(instance, Enum.ToObject(f.FieldType, 2));
                            break;
                        }
                        if (f.FieldType == typeof(int))
                        {
                            f.SetValue(instance, 2);
                            break;
                        }
                    }
                    arr.SetValue(instance, i);
                }
            }
        }

        private static void SetAllBoolFlags(SystemData sd, string fieldName)
        {
            var field = AccessTools.Field(typeof(SystemData), fieldName);
            var arr = field?.GetValue(sd) as bool[];
            if (arr == null) return;

            for (int i = 0; i < arr.Length; i++)
                arr[i] = true;
        }
    }

    // ─── Harmony Patches ──────────────────────────────────────────

    [HarmonyPatch(typeof(Saves), "Load")]
    public static class SavesLoadPatch
    {
        static void Postfix(Saves __instance)
        {
            CheatController.SavesRef = __instance;
            Plugin.Log.LogInfo("[Cheat] Saves reference captured (Load)");
        }
    }

    [HarmonyPatch(typeof(GameData), "Initialize")]
    public static class GameDataInitPatch
    {
        static void Postfix(GameData __instance)
        {
            Plugin.Log.LogInfo("[Cheat] GameData.Initialize() detected");
        }
    }
}
