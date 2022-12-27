using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace VAT.Packaging.Editor {
    public static class AddressablesManager {
        private static AddressableAssetSettings _loadedSettings = null;
        public static AddressableAssetSettings LoadedSettings {
            get {
                if (_loadedSettings == null) {
                    var defaultSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                    _loadedSettings = defaultSettings;
                }

                return _loadedSettings;
            }
        }

        public static void SetActiveSettings() {
            var settings = LoadedSettings;

            if (settings != null) {
                settings.activeProfileId = settings.profileSettings.GetProfileId("Default");
            }
        }

        public static void ClearGroups() {
            var settings = LoadedSettings;
            if (settings == null)
                return;

            foreach (var group in settings.groups.ToArray())
                settings.RemoveGroup(group);

            AssetDatabase.SaveAssets();
        }
    }
}
