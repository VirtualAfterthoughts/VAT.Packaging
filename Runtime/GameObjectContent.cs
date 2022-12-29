using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public class GameObjectContent : ContentT<GameObject> {
        [SerializeField]
        protected new CrystGameObject _mainAsset;

        public override CrystAsset MainAsset {
            get {
                return _mainAsset;
            }
            set {
                if (value != null && value.GetType() == typeof(CrystAsset)) {
                    _mainAsset = new CrystGameObject(value.AssetGUID);

#if UNITY_EDITOR
                    _mainAsset.ValidateGUID(value.EditorAsset);
#endif
                }
                else {
                    _mainAsset = value as CrystGameObject;
                }
            }
        }

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            _mainAsset.ValidateGUID();
            base._mainAsset = _mainAsset;
        }

        public override void SetAsset(Object asset) {
            if (asset is not GameObject)
                throw new ArgumentException("Asset for content was not a GameObject.");

            MainAsset = new CrystGameObject(asset as GameObject);
        }
#endif
    }
}
