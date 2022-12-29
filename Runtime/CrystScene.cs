using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VAT.Packaging
{
    [Serializable]
    public class CrystScene : CrystAsset {
        public CrystScene(string guid) : base(guid) { }

#if UNITY_EDITOR
        public CrystScene(SceneAsset asset) {
            ValidateGUID(asset);
        }

        public new SceneAsset EditorAsset { get { return _editorAsset as SceneAsset; } set { _editorAsset = value; } }
        public override Type AssetType => typeof(SceneAsset);
#endif
    }
}
