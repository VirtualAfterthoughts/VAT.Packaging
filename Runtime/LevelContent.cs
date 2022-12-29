using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Shared.Extensions;

using Object = UnityEngine.Object;

namespace VAT.Packaging {
    public class LevelContent : Content {
        [SerializeField]
        private CrystScene _mainScene;

        public override CrystAsset MainAsset {
            get {
                return _mainScene;
            }
            set {
                if (value != null && value.GetType() == typeof(CrystAsset)) {
                    _mainScene = new CrystScene(value.AssetGUID);
                }
                else {
                    _mainScene = value as CrystScene;
                }
            }
        }

        public CrystScene MainScene { get { return _mainScene; } set { _mainScene = value; } }

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            string path = AssetDatabase.GUIDToAssetPath(MainScene.AssetGUID);
            if (!string.IsNullOrEmpty(path))
                MainScene.EditorAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (isBuilding && MainScene != null && MainScene.EditorAsset && Package != null) {
                var groupName = $"{Address.CleanAddress(Package.Title.ToLower())}_levels";

                MainScene.EditorAsset.MarkAsAddressable(groupName);
            }
        }

        public override void SetAsset(Object asset)
        {
            if (asset is not SceneAsset)
                throw new ArgumentException("Asset for content was not a Scene.");

            MainScene = new CrystScene(asset as SceneAsset);
        }

        public void ForceAsset(SceneAsset asset) {
            var prevAsset = MainScene.EditorAsset;

            MainScene.EditorAsset = asset;
            MainScene.ValidateGUID(asset);

            if (prevAsset != asset)
                EditorUtility.SetDirty(this);
        }
#endif
    }
}
