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
        protected new CrystScene _mainAsset;

        public CrystScene MainScene => _mainAsset;

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            string path = AssetDatabase.GUIDToAssetPath(_mainAsset.AssetGUID);
            if (!string.IsNullOrEmpty(path))
                _mainAsset.EditorAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            base._mainAsset = _mainAsset;

            if (isBuilding && _mainAsset != null && _mainAsset.EditorAsset && Package != null) {
                var groupName = $"{Address.CleanAddress(Package.Title.ToLower())}_levels";

                _mainAsset.EditorAsset.MarkAsAddressable(groupName);
            }
        }

        public override void SetAsset(Object asset)
        {
            if (asset is not SceneAsset)
                throw new ArgumentException("Asset for content was not a Scene.");

            _mainAsset = new CrystScene(asset as SceneAsset);
        }

        public void ForceAsset(SceneAsset asset) {
            var prevAsset = _mainAsset.EditorAsset;

            _mainAsset.EditorAsset = asset;
            _mainAsset.ValidateGUID(asset);

            if (prevAsset != asset)
                EditorUtility.SetDirty(this);
        }
#endif
    }
}
