using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public class ScriptableObjectContent : ContentT<GameObject> {
        [SerializeField]
        private CrystScriptableObject _mainScriptableObject;

        public override CrystAsset MainAsset {
            get {
                return _mainScriptableObject;
            }
            set {
                if (value != null && value.GetType() == typeof(CrystAsset)) {
                    _mainScriptableObject = new CrystScriptableObject(value.AssetGUID);
                }
                else {
                    _mainScriptableObject = value as CrystScriptableObject;
                }
            }
        }

        public CrystScriptableObject MainScriptableObject { get { return _mainScriptableObject; } set { _mainScriptableObject = value; } }

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            _mainScriptableObject.ValidateGUID();
        }

        public override void SetAsset(Object asset) {
            if (asset is not ScriptableObject)
                throw new ArgumentException("Asset for content was not a ScriptableObject.");

            MainAsset = new CrystScriptableObject(asset as ScriptableObject);
        }
#endif
    }
}
