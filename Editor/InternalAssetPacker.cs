using UnityEditor;
using UnityEngine;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public static class InternalAssetPacker {
        [MenuItem("VAT/Internal/Pack Text Assets")]
        public static void PackTextAssets() {
            // Refresh the asset packager
            AssetPackager.EditorForceRefresh();

            // Set the addressables info
            AddressablesManager.ClearGroups();
            InternalAddressablesManager.SetActiveSettings();

            // Verify text asset folder
            var path = $"Assets/{AssetPackager.CRYST_ASSETS_FOLDER}/{AssetPackager.CRYST_TEXT_ASSETS_FOLDER}";
            if (AssetDatabase.IsValidFolder(path))
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateFolder($"Assets/{AssetPackager.CRYST_ASSETS_FOLDER}", AssetPackager.CRYST_TEXT_ASSETS_FOLDER);

            // Save all packages as text assets
            foreach (var package in AssetPackager.Instance.GetPackages()) {
                if (!package.Internal)
                    continue;

                var packer = new JSONPacker();
                var json = packer.PackRoot(package);
                TextAsset textAsset = new(json.ToString());

                AssetDatabase.CreateAsset(textAsset, $"{path}/{package.Title}.asset");

                textAsset.MarkAsAddressable(AssetPackager.INTERNAL_PACKAGES_GROUP, package.Title, AssetPackager.INTERNAL_PACKAGES_LABEL);
            }
        }
    }
}
