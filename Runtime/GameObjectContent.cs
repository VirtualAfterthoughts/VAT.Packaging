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
        private CrystGameObject _mainGameObject;

        public override CrystAsset MainAsset {
            get {
                return _mainGameObject;
            }
            set {
                if (value != null && value.GetType() == typeof(CrystAsset)) {
                    _mainGameObject = new CrystGameObject(value.AssetGUID);
                }
                else {
                    _mainGameObject = value as CrystGameObject;
                }
            }
        }

        public CrystGameObject MainGameObject { get { return _mainGameObject; } set { _mainGameObject = value; } }

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            _mainGameObject.ValidateGUID();
        }

        public override void SetAsset(Object asset) {
            if (asset is not GameObject)
                throw new ArgumentException("Asset for content was not a GameObject.");

            MainAsset = new CrystGameObject(asset as GameObject);
        }
#endif
    }
}
