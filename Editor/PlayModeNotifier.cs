using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace VAT.Packaging.Editor
{
    [InitializeOnLoad]
    public static class PlayModeNotifier {
        static PlayModeNotifier() {
            EditorApplication.playModeStateChanged += Fire;
        }

        private static void Fire(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredEditMode) {
                Debug.Log("Entered Edit Mode, refreshing AssetPackager.");
                AssetPackager.EditorForceRefresh();
            }
        }
    }
}
