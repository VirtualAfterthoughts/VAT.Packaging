using System;

using UnityEditor;

using UnityEngine;

namespace VAT.Packaging.Editor {
    public class AssetPackagerEditor : EditorWindow {
        [MenuItem("VAT/Cryst SDK/Asset Packager", priority = -10000)]
        public static void Initialize() {
            AssetPackagerEditor window = EditorWindow.GetWindow<AssetPackagerEditor>(true, "Asset Packager");
            window.Show();
        }

        public void OnGUI() {
            EditorGUILayout.LabelField("Status", EditorStyles.whiteLargeLabel);

            if (AssetPackager.IsReady) {
                GUILayout.Label("AssetPackager is ready!");
            }
            else {
                GUILayout.Label("AssetPackager is not ready.");
            }

            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            if (GUILayout.Button("Create New Package", GUILayout.Height(20), GUILayout.Width(200))) {
                PackageCreationWizard.Initialize();
            }

            if (GUILayout.Button("Pack all Packages for PC", GUILayout.Height(20), GUILayout.Width(200))) {
                ExternalAssetPacker.PackPackages(BuildTarget.StandaloneWindows64);
            }
        }
    }
}