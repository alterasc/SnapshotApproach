using Kingmaker.Localization;
using ModMenu.Settings;

namespace SnapshotApproach
{
    internal class SettingsModMenu
    {
        private static readonly string RootKey = "AlterAsc.SnapshotApproach".ToLower();
        internal void Initialize()
        {
            ModMenu.ModMenu.AddSettings(
              SettingsBuilder
                .New(GetKey("title"), CreateString("title", "Snapshot Approach"))
                .AddSliderInt(SliderInt.New(GetKey("snapshot"), 5, CreateString("snapshot", "Snap Shot approach range"), 0, 30))
                .AddSliderInt(SliderInt.New(GetKey("improvedsnapshot"), 10, CreateString("improvedsnapshot", "Improved Snap Shot approach range"), 0, 30))
                .AddSliderInt(SliderInt.New(GetKey("greatersnapshot"), 15, CreateString("greatersnapshot", "Greater Snap Shot approach range"), 0, 30))
                .AddToggle(Toggle.New(GetKey("rangedthreatfix"), true, CreateString("rangedthreatfix", "Fix base snap shot range being affected by size"))
                .WithLongDescription(CreateString("rangedthreatfix.desc", "In base game snap shot range is not 5 feet but instead is taken as base threat range, which is modified by character size. This changes calculation for snap shot base threat range to be fixed 5 feet regardless of character size")))
            );
        }

        public int SnapshotRange => ModMenu.ModMenu.GetSettingValue<int>(GetKey("snapshot"));
        public int ImprovedSnapshotRange => ModMenu.ModMenu.GetSettingValue<int>(GetKey("improvedsnapshot"));
        public int GreaterSnapshotRange => ModMenu.ModMenu.GetSettingValue<int>(GetKey("greatersnapshot"));
        public bool FixSnapshotRange => ModMenu.ModMenu.GetSettingValue<bool>(GetKey("rangedthreatfix"));

        private static LocalizedString CreateString(string partialKey, string text)
        {
            return CreateLocalizedString(GetKey(partialKey, "--"), text);
        }

        private static string GetKey(string partialKey, string separator = ".")
        {
            return $"{RootKey}{separator}{partialKey}";
        }

        private static LocalizedString CreateLocalizedString(string key, string value)
        {
            var localizedString = new LocalizedString() { m_Key = key };
            LocalizationManager.CurrentPack.PutString(key, value);
            return localizedString;
        }
    }
}
