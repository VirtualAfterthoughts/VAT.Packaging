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

#if UNITY_EDITOR
        public override void ValidateAsset() {
            _mainAsset.ValidateGUID();
            base._mainAsset = _mainAsset;
        }

        public override void SetAsset(Object asset) {
            if (asset is not GameObject)
                throw new ArgumentException("Asset for content was not a GameObject.");

            _mainAsset = new CrystGameObject(asset as GameObject);
        }
#endif
    }
}
