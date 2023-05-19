using HarmonyLib;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items.Slots;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using UnityModManagerNet;

namespace SnapshotApproach
{
#if DEBUG
    [EnableReloading]
#endif
    static class Main
    {
        public static bool Enabled;
        public static UnityModManager.ModEntry ModEntry;
        public static SettingsModMenu Settings;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = new SettingsModMenu();
            var harmony = new Harmony(modEntry.Info.Id);
            ModEntry = modEntry;
            modEntry.OnToggle = OnToggle;
#if DEBUG
            modEntry.OnUnload = OnUnload;
#endif
            harmony.PatchAll();
            return true;
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            return true;
        }
    }

    internal class SettingsStarter
    {
        [HarmonyPatch(typeof(BlueprintsCache), nameof(BlueprintsCache.Init))]
        internal static class BlueprintsCache_Init_Patch
        {
            private static bool _initialized;

            [HarmonyPostfix]
            static void Postfix()
            {
                if (_initialized) return;
                _initialized = true;
                Main.Settings.Initialize();
            }
        }
    }

    [HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.GetWeaponRange))]
    static class UnitDescriptor_GetWeaponRange_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Feet __result, UnitDescriptor __instance, BlueprintItemWeapon weapon)
        {
            if (__instance.State.Features.PointBlankRange)
            {
                if (!__instance.State.Features.SnapShot)
                {
                    return;
                }
                if (__instance.State.Features.GreaterSnapShot)
                {
                    var range = Main.Settings.GreaterSnapshotRange;
                    if (range < __result.Value)
                    {
                        __result = range.Feet();
                    }
                }
                else if (__instance.State.Features.ImprovedSnapShot)
                {
                    var range = Main.Settings.ImprovedSnapshotRange;
                    if (range < __result.Value)
                    {
                        __result = range.Feet();
                    }
                }
                else
                {
                    var range = Main.Settings.SnapshotRange;
                    if (range < __result.Value)
                    {
                        __result = range.Feet();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UnitHelper), nameof(UnitHelper.GetThreatRange))]
    static class UnitHelper_GetThreatRange_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref float? __result, UnitDescriptor __instance, UnitEntityData unit, WeaponSlot hand)
        {
            if (!hand.Weapon.Blueprint.IsRanged || !unit.State.Features.SnapShot || !Main.Settings.FixSnapshotRange) return;
            __result = 5.Feet().Meters + (unit.State.Features.ImprovedSnapShot ? (5.Feet().Meters + (unit.State.Features.GreaterSnapShot ? 5.Feet().Meters : 0f)) : 0f);
        }
    }
}
